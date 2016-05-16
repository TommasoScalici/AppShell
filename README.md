# AppShell
AppShell provides a template for building UWP apps with a complete, standard and fully customizable "hamburger menu", that is built using SplitView control.
This avoid you to build or copy/paste the menu for each app without sacrificing flexibility.


## How to use

To start using AppShell, you can just install the NuGet Package: [AppShell](https://www.nuget.org/packages/TommasoScalici.AppShell)

Then, all you have to do is to insert the AppShell control in your MainPage.xaml and insert at least one MenuListView (with some MenuItems) in the Menu of the AppShell.

Here is an example:

![alt tag](http://i.imgur.com/OkxA7w7.png "AppShell sample 1")
![alt tag](http://i.imgur.com/u2nTADP.png "AppShell sample 2")

And here's the code you need to achieve that result:

```xaml
<Page x:Class="TestApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:as="using:TommasoScalici.AppShell"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:local="using:TestApp"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
      mc:Ignorable="d">

    <as:AppShell>

        <as:AppShell.Menu>

            <Grid>

                <Grid.RowDefinitions>
                    <RowDefinition />
                    <RowDefinition Height="Auto" />
                </Grid.RowDefinitions>

                <as:MenuListView Margin="0,48,0,0">
                    <as:MenuItem Label="Home" Symbol="Home" />
                    <as:MenuItem Label="Profile" Symbol="ContactInfo" />
                    <as:MenuItem Label="Info" SymbolAsGlyph="&#xE946;" />
                </as:MenuListView>

                <as:MenuListView Grid.Row="1">
                    <as:MenuItem IsSelectable="False"
                                 Label="Username"
                                 SymbolVisibility="Collapsed">
                        <as:MenuItem.UIContent>
                            <Ellipse Width="38"
                                     Height="38"
                                     Margin="5,0,10,0">
                                <Ellipse.Fill>
                                    <ImageBrush ImageSource="Assets/UserImage.png" />
                                </Ellipse.Fill>
                            </Ellipse>
                        </as:MenuItem.UIContent>
                    </as:MenuItem>
                    <as:MenuItem Label="Settings" Symbol="Setting" />
                </as:MenuListView>

            </Grid>
        </as:AppShell.Menu>
    </as:AppShell>

</Page>
```


Each MenuItem object has different properties and events that you can use to customize both the aspect and the user interaction:

- **Arguments** The argument to pass for a Page Navigation.
- **Click** An event handler for click on the MenuItem.
- **DestinationPage** The Page to navigate when the MenuItem is selected. You must write the full type name (this means including the full namespace).
- **IsSelectable** False if MenuItem can't be selected, True otherwise. Usually you want selection for MenuItems that navigate to pages and you won't it for MenuItems that open dialogs or do some other kind of actions.
- **Label** The text associated with the MenuItem.
- **Symbol / SymbolAsGlyph** The SymbolIcon associated with the MenuItem. You should set only one of these properties at once. Symbol will use the [Symbol Enum](https://msdn.microsoft.com/library/windows/apps/windows.ui.xaml.controls.symbol.aspx) and SymbolAsGlyph is for using a XAML character entity.
- **UIContent** An optional UIElement of your choose to associate with the MenuItem.
- **LabelVisibility / SymbolVisibilty / UIContentVisibility** Set the Visibility of the relative content.



If you want to use also the PageHeader and PageFooter feature in your pages, the implementation is straightforward:

```xaml
<Page x:Class="AppShell.TestApp.TestPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:as="using:TommasoScalici.AppShell"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:local="using:AppShell.TestApp"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">

    <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">

        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <as:PageHeader Grid.Row="0">
            <as:PageHeader.HeaderContent>
                <TextBlock HorizontalAlignment="Center"
                           Style="{ThemeResource SubheaderTextBlockStyle}"
                           Text="Header" />
            </as:PageHeader.HeaderContent>
        </as:PageHeader>

        <TextBlock Grid.Row="1"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Style="{ThemeResource HeaderTextBlockStyle}"
                   Text="Here goes your content" />

        <as:PageFooter Grid.Row="2">
            <TextBlock HorizontalAlignment="Center"
                       Style="{ThemeResource SubheaderTextBlockStyle}"
                       Text="Footer" />
        </as:PageFooter>

    </Grid>
</Page>
```


## License

AppShell is released under [MIT License](https://github.com/TommasoScalici/AppShell/blob/master/LICENSE.md).
