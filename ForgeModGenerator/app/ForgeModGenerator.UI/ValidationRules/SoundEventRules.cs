using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.ValidationRules
{
    public class SoundEventValidationDependencyWrapper : DependencyObject
    {
        public static readonly DependencyProperty SoundEventBeforeChangeProperty =
            DependencyProperty.Register("SoundEventBeforeChange", typeof(SoundEvent), typeof(SoundEventValidationDependencyWrapper), new PropertyMetadata(null));
        public SoundEvent SoundEventBeforeChange {
            get { return (SoundEvent)GetValue(SoundEventBeforeChangeProperty); }
            set { SetValue(SoundEventBeforeChangeProperty, value); }
        }

        public static readonly DependencyProperty SoundEventsProperty =
            DependencyProperty.Register("SoundEvents", typeof(IEnumerable<SoundEvent>), typeof(SoundEventValidationDependencyWrapper), new PropertyMetadata(null));
        public IEnumerable<SoundEvent> SoundEvents {
            get { return (IEnumerable<SoundEvent>)GetValue(SoundEventsProperty); }
            set { SetValue(SoundEventsProperty, value); }
        }
    }

    public class SoundEventRules : ValidationRule
    {
        public SoundEventRules() : base() { }
        public SoundEventRules(ValidationStep validationStep, bool validatesOnTargetUpdated) : base(validationStep, validatesOnTargetUpdated) { }
        public SoundEventRules(string propertyName, SoundEventValidationDependencyWrapper parameters)
        {
            PropertyName = propertyName;
            Parameters = parameters;
        }

        public string PropertyName { get; set; }
        public SoundEventValidationDependencyWrapper Parameters { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (PropertyName)
            {
                case nameof(SoundEvent.EventName):
                    return ValidateEventName(value.ToString());
                default:
                    throw new NotImplementedException($"Validation of paremeter: {PropertyName} is not implemented");
            }
        }

        private ValidationResult ValidateEventName(string eventName)
        {
            ValidationResult emptyResult = new NotEmptyRule().Validate(eventName, null);
            if (!emptyResult.IsValid)
            {
                return emptyResult;
            }
            if (Parameters == null || Parameters.SoundEvents == null || Parameters.SoundEventBeforeChange == null)
            {
                return new ValidationResult(false, "Validation failed, one of parameters was null");
            }
            if (Parameters.SoundEventBeforeChange.EventName != eventName)
            {
                int eventNameCount = Parameters.SoundEvents.Count(x => x.EventName == eventName) + 1;
                ValidationResult existResult = new ValidationResult(eventNameCount <= 1, $"{eventName} already exists");
                if (!existResult.IsValid)
                {
                    return existResult;
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
