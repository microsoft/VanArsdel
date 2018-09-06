// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace VanArsdel
{
    public interface IContextActionProvider
    {
        void HandleDeviceContextRequested(Devices.Device device, UIElement targetElement, Point point);
        void ShowRenameFlyout(FrameworkElement attachTo, string startingValue, Action<string> onComplete);
        void ShowNewRoomFlyout(FrameworkElement attachTo, string startingValue, Action<string> onComplete);

        void HandleDeleteRequested(Devices.Device device);
        void HandleRenameRequested(Devices.Device device, FrameworkElement container);
        void HandleMoveRequested(Devices.Device device, FrameworkElement targetElement);
    }

    public class ContextActionProvider : IContextActionProvider
    {
        public ContextActionProvider(IStringProvider stringProvider, IDeviceProvider deviceProvider)
        {
            _stringProvider = stringProvider;
            _deviceProvider = deviceProvider;
        }

        private IStringProvider _stringProvider;
        private IDeviceProvider _deviceProvider;

        public void HandleDeviceContextRequested(Devices.Device device, UIElement targetElement, Point point)
        {
            _deviceProvider.SelectDevices(new Devices.Device[] { device });

            MenuFlyout flyout = new MenuFlyout();

            MenuFlyoutItem renameItem = new MenuFlyoutItem();
            renameItem.Text = _stringProvider.GetString("DeviceContextMenuItemRename");
            renameItem.Tag = new Tuple<Devices.Device, UIElement>(device, targetElement);
            renameItem.Icon = new SymbolIcon(Symbol.Rename);
            renameItem.Click += RenameItem_Click;
            flyout.Items.Add(renameItem);

            MenuFlyoutItem deleteItem = new MenuFlyoutItem();
            deleteItem.Text = _stringProvider.GetString("DeviceContextMenuItemRemove");
            deleteItem.Tag = new Tuple<Devices.Device, UIElement>(device, targetElement);
            deleteItem.Icon = new SymbolIcon(Symbol.Remove);
            deleteItem.Click += DeleteItem_Click;
            flyout.Items.Add(deleteItem);

            MenuFlyoutSubItem moveItem = new MenuFlyoutSubItem();
            moveItem.Text = _stringProvider.GetString("DeviceContextMenuItemMove");
            moveItem.Icon = new SymbolIcon(Symbol.MoveToFolder);
            flyout.Items.Add(moveItem);

            foreach (var room in _deviceProvider.Rooms)
            {
                if (room.Id != device.Room.Id)
                {
                    MenuFlyoutItem roomItem = new MenuFlyoutItem();
                    roomItem.Text = room.Caption;
                    roomItem.Tag = new Tuple<Devices.Device, Devices.Room>(device, room);
                    roomItem.Click += OnMoveDeviceToRoom;
                    moveItem.Items.Add(roomItem);
                }
            }

            flyout.ShowAt(targetElement, point);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement source)
            {
                if (source.Tag is Tuple<Devices.Device, UIElement> args)
                {
                    var device = args.Item1;
                    HandleDeleteRequested(device);
                }
            }
        }

        public void HandleDeleteRequested(Devices.Device device)
        {
            _deviceProvider.RemoveDevice(device);
        }

        private void RenameItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement source)
            {
                if (source.Tag is Tuple<Devices.Device, UIElement> args)
                {
                    var device = args.Item1;
                    var container = args.Item2;
                    HandleRenameRequested(device, container as FrameworkElement);
                }
            }
        }

        public void HandleRenameRequested(Devices.Device device, FrameworkElement container)
        {
            ShowRenameFlyout(container, device.Caption, (name) =>
            {
                if (!string.IsNullOrEmpty(name))
                {
                    _deviceProvider.RenameDevice(device, name);
                }
            });
        }

        private void OnMoveDeviceToRoom(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement source)
            {
                if (source.Tag is Tuple<Devices.Device, Devices.Room> moveArgs)
                {
                    _deviceProvider.MoveDeviceToRoom(moveArgs.Item1, moveArgs.Item2);
                }
            }
        }

        public void ShowRenameFlyout(FrameworkElement attachTo, string startingValue, Action<string> onComplete)
        {
            Flyout renameFlyout = new Flyout();

            Grid grid = new Grid();
            grid.MinWidth = 320;
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.Tag = onComplete;

            TextBox inputBox = new TextBox();
            inputBox.Name = "InputBox";
            inputBox.Text = startingValue;
            inputBox.SetValue(Grid.RowProperty, 0);
            inputBox.Margin = new Thickness(16, 0, 16, 8);
            inputBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            inputBox.VerticalAlignment = VerticalAlignment.Top;
            inputBox.KeyUp += RenameInputBox_KeyUp;
            inputBox.Header = _stringProvider.GetString("NameFlyoutInputHeader");
            inputBox.Tag = renameFlyout;
            grid.Children.Add(inputBox);

            Button okButton = new Button();
            okButton.Name = "OkButton";
            okButton.SetValue(Grid.RowProperty, 1);
            okButton.MinWidth = 100;
            okButton.Margin = new Thickness(0, 0, 16, 0);
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.VerticalAlignment = VerticalAlignment.Top;
            okButton.Content = _stringProvider.GetString("NameFlyoutOkButtonContent");
            okButton.Click += RenameOkButton_Click;
            okButton.Tag = renameFlyout;
            grid.Children.Add(okButton);

            renameFlyout.Content = grid;

            renameFlyout.ShowAt(attachTo);
        }

        private void RenameOkButton_Click(object sender, RoutedEventArgs e)
        {
            Button okButton = sender as Button;
            Flyout sourceFlyout = okButton.Tag as Flyout;
            Grid sourceGrid = sourceFlyout.Content as Grid;
            Action<string> handler = sourceGrid.Tag as Action<string>;
            TextBox inputBox = VanArsdel.Utils.VisualTreeHelpers.FindElementByName<TextBox>(sourceGrid, "InputBox");
            inputBox.KeyUp -= RenameInputBox_KeyUp;
            okButton.Click -= RenameOkButton_Click;
            sourceFlyout.Hide();
            var input = inputBox.Text;
            if (!string.IsNullOrEmpty(input))
            {
                handler?.Invoke(input);
            }
        }

        private void RenameInputBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TextBox inputBox = sender as TextBox;
                Flyout sourceFlyout = inputBox.Tag as Flyout;
                Grid sourceGrid = sourceFlyout.Content as Grid;
                Action<string> handler = sourceGrid.Tag as Action<string>;
                Button okButton = VanArsdel.Utils.VisualTreeHelpers.FindElementByName<Button>(sourceGrid, "OkButton");
                inputBox.KeyUp -= RenameInputBox_KeyUp;
                okButton.Click -= RenameOkButton_Click;
                sourceFlyout.Hide();
                var input = inputBox.Text;
                if (!string.IsNullOrEmpty(input))
                {
                    handler?.Invoke(input);
                }
            }
        }

        public void ShowNewRoomFlyout(FrameworkElement attachTo, string startingValue, Action<string> onComplete)
        {
            Flyout renameFlyout = new Flyout();

            Grid grid = new Grid();
            grid.MinWidth = 320;
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.Tag = onComplete;

            ComboBox inputBox = new ComboBox();
            inputBox.Name = "InputBox";
            inputBox.SetValue(Grid.RowProperty, 0);
            inputBox.IsEditable = true;
            inputBox.ItemsSource = _stringProvider.GetString("RoomNameSuggestions").Split(',');
            inputBox.Margin = new Thickness(16, 0, 16, 8);
            inputBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            inputBox.VerticalAlignment = VerticalAlignment.Top;
            inputBox.Header = _stringProvider.GetString("NameFlyoutInputHeader");
            grid.Children.Add(inputBox);

            Button okButton = new Button();
            okButton.Name = "OkButton";
            okButton.SetValue(Grid.RowProperty, 1);
            okButton.MinWidth = 100;
            okButton.Margin = new Thickness(0, 0, 16, 0);
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.VerticalAlignment = VerticalAlignment.Top;
            okButton.Content = _stringProvider.GetString("NameFlyoutOkButtonContent");
            okButton.Click += NewRoomOkButton_Click;
            okButton.Tag = renameFlyout;
            grid.Children.Add(okButton);

            renameFlyout.Content = grid;
            renameFlyout.ShowAt(attachTo);
        }

        private void NewRoomOkButton_Click(object sender, RoutedEventArgs e)
        {
            Button okButton = sender as Button;
            Flyout sourceFlyout = okButton.Tag as Flyout;
            Grid sourceGrid = sourceFlyout.Content as Grid;
            Action<string> handler = sourceGrid.Tag as Action<string>;
            ComboBox inputBox = VanArsdel.Utils.VisualTreeHelpers.FindElementByName<ComboBox>(sourceGrid, "InputBox");
            okButton.Click -= RenameOkButton_Click;
            sourceFlyout.Hide();
            var input = inputBox.SelectedValue as string;
            if (!string.IsNullOrEmpty(input))
            {
                handler?.Invoke(input);
            }
        }

        public void HandleMoveRequested(Devices.Device device, FrameworkElement targetElement)
        {
            MenuFlyout flyout = new MenuFlyout();

            foreach (var room in _deviceProvider.Rooms)
            {
                if (room.Id != device.Room.Id)
                {
                    MenuFlyoutItem roomItem = new MenuFlyoutItem();
                    roomItem.Text = room.Caption;
                    roomItem.Tag = new Tuple<Devices.Device, Devices.Room>(device, room);
                    roomItem.Click += OnMoveDeviceToRoom;
                    flyout.Items.Add(roomItem);
                }
            }

            flyout.ShowAt(targetElement);
        }
    }
}
