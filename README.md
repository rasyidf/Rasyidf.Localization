# Rasyidf Localization

Fast and simple localization framework.

## Getting Started

### Installation
To use this framework. add nuget package:

```powershell
Install-Package Rasyidf.Localization
```

Then register the the services on App.cs

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

### Usage

All set, now you can implement Binding in any XAML like this:

``` xml 
<!-- Bind like this-->
<MenuItem Header="{ul:Tr File, Uid=11}"/>

<!--Or this -->
<TextBlock Text="{ul:Tr Default='Default Hello World', Uid=11}"/>
<TextBlock Text="{ul:Tr FallBackText, Uid=12}"/>
<!-- Or this, with Format String -->
<TextBlock>
	<Run>
		<ul:Tr Uid="24" Default="Language : {0}, Count : {1} ">
			<Binding FallbackValue="en-US" Mode="OneWay"
				Path="CurrentLanguage" />
			<Binding FallbackValue="0" Mode="OneWay"
				Path="LanguageCount" />
		</ul:Tr>
	</Run>
</TextBlock>

```

or you can call in code behind

```csharp
	MessageBox.Show(LocalizationService.GetString("511", "Text", "Default Message"),LocalizationService.GetString("511", "Header","Default Title"));
```

### Language Packs

The Language Pack can be XML or JSON like below, put in the language folder:

In XML
```xml
<Pack EnglishName="English" CultureName="English" Culture="en-US">
  <Value Id="0" Header="Window Title" />
  <Value Id="11" Header="File" />
  <Value Id="110" Header="Exit" /> 
</Pack>
```

Or In Json
```json
{
    "EnglishName": "Indonesian", "CultureName": "Bahasa Indonesia",
    "Culture": "id-ID",
    "Data": [ 
    { "Id": 0, "Header": "Judul Jendela" }, 
    { "Id": 11, "Header": "Berkas" }, 
    { "Id": 110, "Header": "Keluar" }
    ]
}
```

All Done :)


## Authors

* **Rasyid, Muhammad Fahmi** - *Initial work* - [Rasyidf](https://github.com/rasyidf)

See also the list of [contributors](https://github.com/rasyidf/rasyidf.Localization/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

See list of third party components for [aditional acknowledgements](https://github.com/rasyidf/rasyidf.Localization/wiki/List-of-Contributors)
