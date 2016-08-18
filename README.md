# ModelBinders
C# MVC custom Model Binders

## Usage
Simply add the desired files to your project, rename `MyApp.Models` to your namespace of choice and register them in your `global.asax` file:

```C#
protected void Application_Start()
{
    ModelBinders.Binders[typeof(bool)] = new BoolModelBinder();
    ModelBinders.Binders[typeof(bool?)] = new BoolModelBinder();
    ModelBinders.Binders[typeof(DateTime)] = new DateAndTimeModelBinder();
    ModelBinders.Binders[typeof(DateTime?)] = new DateAndTimeModelBinder();
    ModelBinders.Binders[typeof(decimal)] = new DecimalModelBinder();
    ModelBinders.Binders[typeof(decimal?)] = new DecimalModelBinder();
}
```
