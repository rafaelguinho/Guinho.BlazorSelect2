Select2 component for Blazor applications.
Add using to `_Imports.razor`
```html
@using Guinho.BlazorSelect2
```

# Usage

If simple Select2 usage is needed in side a EditForm component:
```html
<Select2 @bind-Value="dog.Race" 
                 Options="@options">
        </Select2>
```
In the @code section you need to instance a list, this list is where the search will be performed: 
```csharp

//model
Dog dog = new Dog();

List<Select2Option<string>> options = new List<Select2Option<string>>
{
    new Select2Option<string>
    {
        Label="Poodle",
        Value= "1"
    },
    new Select2Option<string>
    {
        Label="Golden Retriever",
        Value= "2"
    },
    new Select2Option<string>
    {
        Label="Beagle",
        Value= "3"
    }
};
```

It also supports complex objects with validation.

See BlazorApp project for more examples.
