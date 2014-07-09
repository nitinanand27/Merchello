﻿namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a customer base class
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class AnonymousCustomerBase : Entity, ICustomerBase
    {
        /// <summary>
        /// The last activity date selector.
        /// </summary>
        private static readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<AnonymousCustomerBase, DateTime>(x => x.LastActivityDate);

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The last activity date.
        /// </summary>
        private DateTime _lastActivityDate;

        /// <summary>
        /// The extended data collection
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerBase"/> class.
        /// </summary>
        /// <param name="isAnonymous">
        /// The is anonymous.
        /// </param>
        protected AnonymousCustomerBase(bool isAnonymous)
            : this(isAnonymous, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerBase"/> class.
        /// </summary>
        /// <param name="isAnonymous">
        /// The is anonymous.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        protected AnonymousCustomerBase(bool isAnonymous, ExtendedDataCollection extendedData)
        {
            IsAnonymous = isAnonymous;
            _extendedData = extendedData;
        }

        /// <summary>
        /// Gets the entity key.
        /// </summary>
        public Guid EntityKey
        {
            get
            {
                return Key;
            }
        }

        /// <summary>
        /// Gets or sets the date the customer was last active on the site
        /// </summary>
        [DataMember]
        public DateTime LastActivityDate
        {
            get
            {
                return _lastActivityDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o => 
                    {
                        _lastActivityDate = value;
                        return _lastActivityDate;
                    }, 
                    _lastActivityDate, 
                    LastActivityDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this customer is an anonymous customer 
        /// </summary>
        [IgnoreDataMember]
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// A collection to store custom/extended data for the customer
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}