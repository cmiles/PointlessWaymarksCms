﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsWpfControls.Utility.ChangesAndValidation;
using TypeSupport.Extensions;

namespace PointlessWaymarks.CmsWpfControls.ConversionDataEntry
{
    public class ConversionDataEntryContext<T> : INotifyPropertyChanged, IHasChanges, IHasValidationIssues
    {
        private Func<T, T, bool> _comparisonFunction;

        private Func<string, (bool passed, string conversionMessage, T result)> _converter;
        private bool _hasChanges;
        private bool _hasValidationIssues;
        private string _helpText;
        private bool _isNumeric;
        private T _referenceValue;
        private string _title;
        private string _userText;
        private T _userValue;

        private List<Func<T, IsValid>> _validationFunctions = new();

        private string _validationMessage;

        private ConversionDataEntryContext()
        {
            ComparisonFunction = (referenceValue, userValue) => referenceValue.Equals(userValue);
            if (typeof(T).GetExtendedType().IsNumericType) IsNumeric = true;
        }

        public Func<T, T, bool> ComparisonFunction
        {
            get => _comparisonFunction;
            set
            {
                if (Equals(value, _comparisonFunction)) return;
                _comparisonFunction = value;
                OnPropertyChanged();
            }
        }

        public Func<string, (bool passed, string conversionMessage, T result)> Converter
        {
            get => _converter;
            set
            {
                if (Equals(value, _converter)) return;
                _converter = value;
                OnPropertyChanged();
            }
        }

        public string HelpText
        {
            get => _helpText;
            set
            {
                if (value == _helpText) return;
                _helpText = value;
                OnPropertyChanged();
            }
        }

        public bool IsNumeric
        {
            get => _isNumeric;
            set
            {
                if (value == _isNumeric) return;
                _isNumeric = value;
                OnPropertyChanged();
            }
        }

        public T ReferenceValue
        {
            get => _referenceValue;
            set
            {
                if (value.Equals(_referenceValue)) return;
                _referenceValue = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string UserText
        {
            get => _userText;
            set
            {
                if (value == _userText) return;
                _userText = value;
                OnPropertyChanged();
            }
        }

        public T UserValue
        {
            get => _userValue;
            private set
            {
                if (value.Equals(_userValue)) return;
                _userValue = value;
                OnPropertyChanged();
            }
        }

        public List<Func<T, IsValid>> ValidationFunctions
        {
            get => _validationFunctions;
            set
            {
                if (Equals(value, _validationFunctions)) return;
                _validationFunctions = value;
                OnPropertyChanged();
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                if (value == _validationMessage) return;
                _validationMessage = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (value == _hasChanges) return;
                _hasChanges = value;
                OnPropertyChanged();
            }
        }

        public bool HasValidationIssues
        {
            get => _hasValidationIssues;
            set
            {
                if (value == _hasValidationIssues) return;
                _hasValidationIssues = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CheckForChangesAndValidate()
        {
            HasChanges = !ComparisonFunction(ReferenceValue, UserValue);

            if (ValidationFunctions != null && ValidationFunctions.Any())
                foreach (var loopValidations in ValidationFunctions)
                {
                    var validationResult = loopValidations(UserValue);
                    if (!validationResult.Valid)
                    {
                        HasValidationIssues = true;
                        ValidationMessage = validationResult.Explanation;
                        return;
                    }
                }

            HasValidationIssues = false;
            ValidationMessage = string.Empty;
        }

        public static ConversionDataEntryContext<T> CreateInstance(
            Func<string, (bool passed, string conversionMessage, T result)> converter)
        {
            return new ConversionDataEntryContext<T> { Converter = converter };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName)) return;

            if (propertyName.Contains(nameof(UserText))) TryConvertUserText();

            if (!propertyName.Contains("HasChanges") && !propertyName.Contains("Validation") &&
                !propertyName.Contains(nameof(UserText)))
                CheckForChangesAndValidate();
        }

        private void TryConvertUserText()
        {
            if (Converter == null)
            {
                HasValidationIssues = true;
                ValidationMessage = "No conversion available";
                return;
            }

            var (passed, conversionMessage, result) = Converter(UserText.TrimNullToEmpty());

            if (!passed)
            {
                HasValidationIssues = true;
                ValidationMessage = conversionMessage;
                return;
            }

            UserValue = result;
        }
    }
}