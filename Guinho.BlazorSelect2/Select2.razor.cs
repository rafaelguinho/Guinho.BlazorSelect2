using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guinho.BlazorSelect2
{
    public class Select2Base<TValue> : InputBase<TValue>, IAsyncDisposable
    {
        [Parameter] public IList<Select2Option<TValue>> Options { get; set; }

        [Parameter] public string Label { get; set; } = "Search";

        [Parameter] public EventCallback<object> OnValueSelected { get; set; }

        [Parameter] public int TabIndex { get; set; } = 0;

        [Parameter]
        public string HtmlCustomLabel { get; set; }

        [Inject]
        private IJSRuntime jsRuntime { get; set; }

        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        protected string InternalId = Guid.NewGuid().ToString();

        protected string UlInternalId = Guid.NewGuid().ToString();

        protected string InputInternalId = Guid.NewGuid().ToString();

        protected IList<Select2Option<TValue>> FilteredOptions;

        protected bool IsOpen = false;

        protected Select2Option<TValue> SelectedOption = null;

        public Select2Base()
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
              "import", "./_content/Guinho.BlazorSelect2/BlazorSelect2.js").AsTask());
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Options != null && Options.Any() && Options.Any(o => o.Selected))
            {
                SelectedOption = Options.First(o => o.Selected);
            }
            else
            {
                SelectedOption = null;
            }

            FilteredOptions = Options;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpen)
            {
                var module = await moduleTask.Value;

                await module.InvokeVoidAsync(
                    "setSelect2Instance",
                    DotNetObjectReference.Create(this), UlInternalId);

                await module.InvokeVoidAsync(
                "setContentSize",
                InternalId);
            }
        }

        private void ResetValues()
        {
            FilteredOptions = Options;
        }

        protected async Task HandleInput(ChangeEventArgs args)
        {
            string value = args.Value.ToString().ToLower();

            FilteredOptions = Options.Where(o => o.Label.ToLower().Contains(value)).ToList();
        }

        protected bool Enabled()
        {
            return Options != null && Options.Any();
        }

        protected async Task OpenOnEnter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !IsOpen && Enabled())
            {
                IsOpen = true;
                await OpenActions();
            }
        }

        protected async Task OpenCloseOptions()
        {
            if (!Enabled())
            {
                return;
            }

            IsOpen = !IsOpen;

            if (IsOpen)
            {
                await OpenActions();
            }
            else
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync(
               "removeEventListener",
               UlInternalId);
            }

            StateHasChanged();
        }

        private async Task OpenActions()
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync(
                           "keyboardsEvent",
                           UlInternalId);

            await module.InvokeVoidAsync(
           "setFocus",
           InputInternalId);

            ResetValues();
        }

        [JSInvokable("Close")]
        public async Task Close(string unlessId)
        {
            if (unlessId != UlInternalId)
            {
                IsOpen = false;

                var module = await moduleTask.Value;
                await module.InvokeVoidAsync(
              "removeEventListener",
              UlInternalId);

                StateHasChanged();
            }

        }

        [JSInvokable("SelectValue")]
        public async Task SelectValue(string optionValue)
        {
            UnselectAll();

            Select2Option<TValue> option = Options.First(p => p.Equals(ChangeType(optionValue)));

            await SelectItem(option);

            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            var module = await moduleTask.Value;
            module.InvokeVoidAsync(
                    "removeSelect2Instance", UlInternalId).GetAwaiter();
        }

        protected async Task SelectValue(Select2Option<TValue> option)
        {
            UnselectAll();

            await SelectItem(option);
        }

        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            result = default(TValue);
            validationErrorMessage = null;
            return true;
        }

        private TValue ChangeType(object value)
        {
            Type t = typeof(TValue);

            string strValue = Convert.ToString(value);

            if (t.IsEnum)
            {
                return (TValue)Enum.Parse(typeof(TValue), strValue, true);
            }
            else if (IsNullableEnum(t))
            {
                return (TValue)Enum.Parse(Nullable.GetUnderlyingType(t), strValue, true);
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(TValue);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (TValue)Convert.ChangeType(value, t);
        }

        private bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        private async Task SelectItem(Select2Option<TValue> option)
        {
            option.Select();
            SelectedOption = option;

            CurrentValue = option.Value;

            if (OnValueSelected.HasDelegate)
            {
                await OnValueSelected.InvokeAsync(CurrentValue);
            }

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(
              "removeEventListener",
              UlInternalId);

            IsOpen = false;
        }

        private void UnselectAll()
        {
            foreach (var opt in Options)
            {
                opt.Unselect();
            }
        }
    }
}
