# Rasyidf Localization
![Nuget](https://img.shields.io/nuget/dt/Rasyidf.Localization)

Fast and simple localization framework for wpf. allow dynamic loading and multiple language pack.

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
            // set the language packs folder and default language
            LocalizationService.Current.Initialize("Assets", "en-US");
        }
        ...
```

### Usage

after installing and loading th assembly, add namespace in the xaml 
``` xml
xmlns:rf="http://rasyidf.github.io/Localisation"
```
All set, now you can implement Binding in any XAML like this:

Bind like this
``` xml 
<MenuItem Header="{rf:Tr File, Uid=11}"/>
```

Or this

``` xml
<TextBlock Text="{rf:Tr Default='Default Hello World', Uid=11}"/>
<TextBlock Text="{rf:Tr FallBackText, Uid=12}"/>
```
Or this, with Format String
``` xml
<TextBlock>
    <Run>
        <rf:Tr Uid="24" Default="Language : {0}, Count : {1} ">
            <Binding FallbackValue="en-US" Mode="OneWay"
                Path="CurrentLanguage" />
            <Binding FallbackValue="0" Mode="OneWay"
                Path="LanguageCount" />
        </rf:Tr>
    </Run>
</TextBlock>
```

in code, you can consume directly by using : 

```csharp
MessageBox.Show(LocalizationService.GetString("511", "Text", "Default Message"),LocalizationService.GetString("511", "Header","Default Title"));
```
another way is to use String extension. `"[uid],[vid]".Localize("[Default]")`

```csharp
MessageBox.Show("511,Text".Localize("Default Message"),("511,Header").Localize("Default Title"));
```

### Language Packs

The Language Pack can be XML or JSON like below, put in the language folder:

> Version 0.5 support some additional metadata (AppId, Author, Type, Version) planned for future version.


#### XML Language Pack


```xml
<Pack Version="0.5" AppId="YourAppId" Author="Rasyid" EnglishName="English" CultureName="English" Culture="en-US">
  <Value Id="0" Header="Window Title" />
  <Value Id="11" Header="File" />
  <Value Id="110" Header="Exit"  /> 
  ...
  <Value Id="511" Header="Hello Wold" Text="This Is message String" />
</Pack>
```
 
#### JSON Language Pack

> Version 0.5 Support Json Multilingual Package.

##### Single Language Pack

```json
{   "AppId" : "YourAppId",
    "Version" : "0.5", "Type" : "Single", 
    "EnglishName": "Indonesian", "CultureName": "Bahasa Indonesia",
    "Culture": "id-ID",
    "Data": [ 
    { "Id": 0, "Header": "Judul Jendela" }, 
    { "Id": 11, "Header": "Berkas" }, 
    { "Id": 110, "Header": "Keluar" }
    ]
}
```

##### Multi Language Pack
```json
{
  "AppId": "",
  "Version": "0.5",
  "Type": "Multi",
  "Author": "Rasyid",
  "Languages": [
    {
      "CultureId": "en-US"  
    },
    {
      "CultureId": "de-DE" 
    }
  ],

  "Data": [
    {
      "data": [
        {
          "Id": 0,
          "Text": {
            "en-US": "Hello", "de-DE": "Hallo"
          }
        }, 
      ]
    }
    ]
  }
}
```

All Done :)

## What is new

0.5.2
* Now rf:Tr will automatically find Uid and Property name based on markup.
* Fixed json stream error when loading metadata. 
* Recreate Demo.WPF because of corrupted file.

0.5.0
* Translation string extension use `"UID, VID".Translate()` to get localized string.
* Multiple Language pack in single file.
* Decouple LanguageStream with Language Item.

0.4.0
* Raw support for xml
* initial json support

## Authors

* **Rasyid, Muhammad Fahmi** - *Initial work* - [Rasyidf](https://github.com/rasyidf)

See also the list of [contributors](https://github.com/rasyidf/rasyidf.Localization/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

See list of third party components for [aditional acknowledgements](https://github.com/rasyidf/rasyidf.Localization/wiki/List-of-Contributors)
