﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PointlessWaymarksCmsWpfControls.TagList
{
    public class TagListListItem : INotifyPropertyChanged
    {
        private int _contentCount;
        private List<TagItemContentInformation> _contentInformation;
        private string _tagName;

        public int ContentCount
        {
            get => _contentCount;
            set
            {
                if (value == _contentCount) return;
                _contentCount = value;
                OnPropertyChanged();
            }
        }

        public List<TagItemContentInformation> ContentInformation
        {
            get => _contentInformation;
            set
            {
                if (Equals(value, _contentInformation)) return;
                _contentInformation = value;
                OnPropertyChanged();
            }
        }

        public string TagName
        {
            get => _tagName;
            set
            {
                if (value == _tagName) return;
                _tagName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}