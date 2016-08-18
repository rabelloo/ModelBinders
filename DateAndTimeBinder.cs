using System;
using System.Web.Mvc;

namespace MyApp.Models
{
    public class DateAndTimeModelBinder : IModelBinder
    {
        public DateAndTimeModelBinder() { }
 
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
 
            // Try to get whole DateTime
            DateTime? dateTimeAttempt = Get<DateTime>(bindingContext, "-DateTime");

            // Return DateTime when it has value
            if (dateTimeAttempt != null)
                return dateTimeAttempt.Value;
 
            // Get Date and Time separately
            DateTime? dateAttempt = Get<DateTime>(bindingContext, this.DateSuffix);
            DateTime? timeAttempt = Get<DateTime>(bindingContext, this.TimeSuffix);
 
            // Return combined Date and Time when both have value
            if (dateAttempt != null && timeAttempt != null)
            {
                return new DateTime(
                                dateAttempt.Value.Year,
                                dateAttempt.Value.Month,
                                dateAttempt.Value.Day,
                                timeAttempt.Value.Hour,
                                timeAttempt.Value.Minute,
                                timeAttempt.Value.Second
                            );
            }

            // Return Date or Time when only one has value
            if (dateAttempt != null)
                return dateAttempt.Value
            if (timeAttempt != null)
                return timeAttempt.Value;

            // Return null when nothing else works
            return null;
        }
 
        private T? Get<T>(ModelBindingContext bindingContext, string suffix) where T : struct
        {
            if (string.IsNullOrEmpty(suffix))
                return null;

            // Try it with the prefix
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + suffix);

            // Try without the prefix if null
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
                valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // If still null return null
            if (valueResult == null)
                return null;

            // Return value
            return (T?)valueResult.ConvertTo(typeof(T), System.Globalization.CultureInfo.CurrentUICulture);
        }

        public string DateSuffix { get; set; }
        public string TimeSuffix { get; set; }
    }
 
    public class DateAndTimeAttribute : CustomModelBinderAttribute
    {
        private IModelBinder _binder;
 
        // The user cares about a full date structure and full
        // time structure, or one or the other.
        public DateAndTimeAttribute(string date = "-Date", string time = "-Time")
        {
            _binder = new DateAndTimeModelBinder
            {
                DateSuffix = date,
                TimeSuffix = time
            };
        }
 
        public override IModelBinder GetBinder() { return _binder; }
    }
}
