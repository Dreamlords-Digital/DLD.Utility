DLD Utility
====

A collection of utility and convenience C# code. This includes:
1. HSBColor: a struct to define color as hue, saturation, and brightness.
2. IoC: a singleton that provides access to all other singletons. IoC will figure out the dependency chain for you, so that by the time your code starts to use any particular singleton, it is guaranteed to have already been initialized.
3. IoC also has simple pooling functionality to let you reuse non-UnityEngine objects. Use this if you are in need of a List, Dictionary, HashSet, etc but want to minimize gc alloc.

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.utility`

DLD Serializer
====

Abstraction on top of an actual serializer so that we can switch to a different implementation with relative ease. Currently, this is designed to use [DLD JsonFx](https://github.com/AnomalousUnderdog/DLD.JsonFx).

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.serializer`

DLD UI Toolkit
====

UI Toolkit convenience functions, plus custom controls for dropdown boxes, context menus, etc. This is intended to replace the DLD IMGUI package.

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.uitoolkit`

DLD IMGUI
====

IMGUI convenience functions for dropdown boxes, color picker widgets, etc. This is designed to work consistently in both Unity Editor and in a runtime build.

This is planned to be removed eventually in favour of the aforementioned DLD UI Toolkit package.

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.imgui`

Easing Equations
====

Animates the value of a float property between two target values using Robert Penner's easing equations (see http://robertpenner.com/easing/).

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.easing`

FuzzyString
====

Approximate String Comparision in C# by kdjones (see https://github.com/kdjones/fuzzystring). Repackaged as a Unity Package.

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.fuzzystring`

Build Info Utility
====

A small utility class that allows you to display the current build's timestamp and git commit hash into the GUI, like in the main menu, for example.

Install via git url: `https://github.com/Dreamlords-Digital/DLD.Utility.git?path=/Packages/com.dreamlordsdigital.buildinfoutil`
