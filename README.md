# Rasyidf Localization

Fast and simple localization framework.

## Getting Started

To use this framework. add nuget pack package:

```powershell
Install-Package Rasyidf.Localization
```

then register the the services on App.cs

```csharp
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LocalizationService.Current.Register("Assets/Languages", "en-US");
        }
        ...
```

then you can implement Binding in any XAML like this:

```xml
<!-- Bind like this-->
<MenuItem Header="{ul:Tr File, Uid=11}"/>
<!--Or this -->
<TextBlock Text="{ul:Tr Default='Default Hello World', Uid=hello}"/>
<TextBlock Text="{ul:Tr Default, Uid=hello}"/>
<!-- Or this, with Format String -->
<Run>
<ul:Tr Uid="24" Default="Language : {0}, Count : {1} ">
    <Binding FallbackValue="en-US" Mode="OneWay"
        Path="CurrentLanguage" />
    <Binding FallbackValue="0" Mode="OneWay"
        Path="LanguageCount" />
</ul:Tr>
</Run>
```

Then create Language Pack like this into the language folder:

In XML
```xml
<Pack EnglishName="English" CultureName="English" Culture="en-US">
  <Value Id="11" Header="File" />
  <Value Id="110" Header="Exit" /> 
</Pack>
```
In Json
```json
{
    "EnglishName": "Javanese", "CultureName": "Basa Jawa",
    "Culture": "jv-Latn-ID",
    "Data": [ 
    { "Id": 0, "Title": "Contoh" }, 
    { "Id": 11, "Header": "Berkas" }
    ]
}
```

And Done :)


## Authors

* **Rasyid, Muhammad Fahmi** - *Initial work* - [Rasyidf](https://github.com/rasyidf)

See also the list of [contributors](https://github.com/rasyidf/UFA.Localization/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

See list of third party components for [aditional acknowledgements](https://github.com/rasyidf/UFA.Localization/wiki/List-of-Contributors)
