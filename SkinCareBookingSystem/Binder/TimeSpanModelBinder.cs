using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Binder
{
    public class TimeSpanModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            if (TimeSpan.TryParse(value, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid TimeSpan format.");
            }

            return Task.CompletedTask;
        }
    }
}
