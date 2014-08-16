using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace AssemblySettings
{
    /// <summary>
    /// Provides access to the settings contained in custom configuration file or in the configuration file of
    /// an assembly.
    /// </summary>
    public class ConfigurationFileApplicationSettings : IComponent, ISite, IServiceProvider
    {
        #region Fields

        Provider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="configuration">The configuration file.</param>
        /// <param name="type">The type that contains the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public ConfigurationFileApplicationSettings(System.Configuration.Configuration configuration, Type type)
            : this(configuration, type == null ? null : type.FullName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="configuration">The configuration file.</param>
        /// <param name="sectionName">The name of the section containing the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sectionName"/> is null or empty.</exception>
        public ConfigurationFileApplicationSettings(System.Configuration.Configuration configuration, string sectionName)
        {
            Init(configuration, sectionName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to load the settings from.</param>
        /// <param name="type">The type that contains the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public ConfigurationFileApplicationSettings(Assembly assembly, Type type)
            : this(assembly, type == null ? null : type.FullName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to load the settings from.</param>
        /// <param name="sectionName">The name of the section containing the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sectionName"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">The assembly has been loaded from the GAC.</exception>
        public ConfigurationFileApplicationSettings(Assembly assembly, string sectionName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("sectionName");
            }

            if (assembly.GlobalAssemblyCache)
            {
                throw new ArgumentException("Assemblies loaded from the GAC are not supported.");
            }

            var path = new Uri(assembly.CodeBase).LocalPath;

            System.Configuration.Configuration configuration;

            if (string.IsNullOrEmpty(assembly.Location))
            {
                // The assembly has been loaded from memory so try to find its configuration file by its name
                // In this case "CodeBase" points to the assembly that loaded the dynamic assembly
                // so expect that the configuration file will be located in its directory.

                // In this case the configuration file must be loaded by its full name because the
                // "ConfigurationManager.OpenExeConfiguration" expects the specified executable file to exists.

                var directoryName = Path.GetDirectoryName(path);

                var extension = assembly.EntryPoint == null ? ".dll" : ".exe";

                path = Path.Combine(directoryName, assembly.GetName().Name + extension + ".config");

                configuration = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap()
                    {
                        ExeConfigFilename = path,
                    },
                    ConfigurationUserLevel.None
                    );
            }
            else
            {
                configuration = ConfigurationManager.OpenExeConfiguration(path);
            }

            Init(configuration, sectionName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="configuration">The path to the configuration file.</param>
        /// <param name="type">The type that contains the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public ConfigurationFileApplicationSettings(string path, Type type)
            : this(path, type == null ? null : type.FullName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileApplicationSettings"/> class.
        /// </summary>
        /// <param name="configuration">The path to the configuration file.</param>
        /// <param name="sectionName">The name of the section containing the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sectionName"/> is null or empty.</exception>
        public ConfigurationFileApplicationSettings(string path, string sectionName)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("sectionName");
            }

            // Convert the path to a full path to avoid looking for the file
            // in the current working directory instead of the binary directory.
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap()
                {
                    ExeConfigFilename = path,
                },
                ConfigurationUserLevel.None
                );

            Init(configuration, sectionName);
        }

        /// <summary>
        /// Inits the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="sectionName">The name of the section containing the settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
        /// <param name="sectionName">The name of the section containing the settings.</param>
        void Init(System.Configuration.Configuration configuration, string sectionName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("sectionName");
            }

            _provider = new Provider(configuration, sectionName);
        }

        #endregion

        #region IComponent Members

        /// <summary>
        /// Represents the method that handles the <see cref="E:System.ComponentModel.IComponent.Disposed"/> event of a component.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets or sets the <see cref="T:System.ComponentModel.ISite"/> associated with the <see cref="T:System.ComponentModel.IComponent"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.ISite"/> object associated with the component; or null, if the component does not have a site.
        /// </returns>
        ISite IComponent.Site
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ISite Members

        /// <summary>
        /// Gets the component associated with the <see cref="T:System.ComponentModel.ISite"/> when implemented by a class.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.IComponent"/> instance associated with the <see cref="T:System.ComponentModel.ISite"/>.
        /// </returns>
        IComponent ISite.Component
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.ComponentModel.IContainer"/> associated with the <see cref="T:System.ComponentModel.ISite"/> when implemented by a class.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.IContainer"/> instance associated with the <see cref="T:System.ComponentModel.ISite"/>.
        /// </returns>
        IContainer ISite.Container
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the component is in design mode when implemented by a class.
        /// </summary>
        /// <value></value>
        /// <returns>true if the component is in design mode; otherwise, false.
        /// </returns>
        bool ISite.DesignMode
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the friendly name used to refer to the provider during configuration.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The friendly name used to refer to the provider during configuration.
        /// </returns>
        string ISite.Name { get; set; }

        #endregion

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(ISettingsProviderService))
            {
                return _provider;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            var temp = Disposed;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Provider class

        class Provider : SettingsProvider, ISettingsProviderService
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of the currently running application.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:System.String"/> that contains the application's shortened name, which does not contain a full path or extension, for example, SimpleAppSettings.
            /// </returns>
            public override string ApplicationName { get; set; }

            #endregion

            #region Fields

            System.Configuration.Configuration _configuration;

            ClientSettingsSection _applicationSection;

            ClientSettingsSection _userSection;

            string _sectionName;

            #endregion

            #region Constructor

            /// <summary>
            /// Inits the specified configuration.
            /// </summary>
            /// <param name="configuration">The configuration.</param>
            /// <param name="sectionName">The name of the section containing the settings.</param>
            /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
            /// <param name="sectionName">The name of the section containing the settings.</param>
            internal Provider(System.Configuration.Configuration configuration, string sectionName)
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }

                if (string.IsNullOrEmpty(sectionName))
                {
                    throw new ArgumentNullException("sectionName");
                }

                _configuration = configuration;

                _sectionName = sectionName;

                var sectionGroup = configuration.GetSectionGroup("applicationSettings");

                _applicationSection = sectionGroup == null ? null : sectionGroup.Sections[_sectionName] as ClientSettingsSection;

                sectionGroup = configuration.GetSectionGroup("userSettings");

                _userSection = sectionGroup == null ? null : sectionGroup.Sections[_sectionName] as ClientSettingsSection;

                // The provider MUST have a name
                Initialize(GetType().Name, null);
            }

            #endregion

            #region Overriden methods

            /// <summary>
            /// Returns the collection of settings property values for the specified application instance and settings property group.
            /// </summary>
            /// <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application use.</param>
            /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyCollection"/> containing the settings property group whose values are to be retrieved.</param>
            /// <returns>
            /// A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> containing the values for the specified settings property group.
            /// </returns>
            public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
            {
                SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

                foreach (SettingsProperty property in collection)
                {
                    var propertyValue = new SettingsPropertyValue(property);

                    var attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;

                    if (attribute != null && attribute.SpecialSetting == SpecialSetting.ConnectionString)
                    {
                        var connectionStrings = _configuration.ConnectionStrings.ConnectionStrings;

                        string connectionStringName = _sectionName + "." + property.Name;

                        if (connectionStrings != null && connectionStrings[connectionStringName] != null)
                        {
                            propertyValue.PropertyValue = connectionStrings[connectionStringName].ConnectionString;
                        }
                        else if (property.DefaultValue != null && property.DefaultValue is string)
                        {
                            propertyValue.PropertyValue = property.DefaultValue;
                        }
                        else
                        {
                            propertyValue.PropertyValue = string.Empty;
                        }

                        propertyValue.IsDirty = false;

                        values.Add(propertyValue);

                        continue;
                    }

                    var section = IsUserSetting(property) ? _userSection : _applicationSection;

                    SettingElement settingElement = null;

                    if (section != null)
                    {
                        foreach (SettingElement item in section.Settings)
                        {
                            if (item.Name == property.Name)
                            {
                                settingElement = item;

                                break;
                            }
                        }
                    }

                    if (settingElement != null)
                    {
                        string innerXml = settingElement.Value.ValueXml.InnerXml;

                        if (settingElement.SerializeAs == SettingsSerializeAs.String)
                        {
                            innerXml = settingElement.Value.ValueXml.InnerText;
                        }

                        propertyValue.SerializedValue = innerXml;
                    }
                    else if (property.DefaultValue != null)
                    {
                        propertyValue.SerializedValue = property.DefaultValue;
                    }
                    else
                    {
                        propertyValue.PropertyValue = null;
                    }

                    propertyValue.IsDirty = false;

                    values.Add(propertyValue);
                }

                return values;
            }

            /// <summary>
            /// Sets the values of the specified group of property settings.
            /// </summary>
            /// <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application usage.</param>
            /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> representing the group of property settings to set.</param>
            /// <exception cref="NotSupportedException">Always.</exception>
            public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region ISettingsProviderService Members

            /// <summary>
            /// Returns the settings provider compatible with the specified settings property.
            /// </summary>
            /// <param name="property">The <see cref="T:System.Configuration.SettingsProperty"/> that requires serialization.</param>
            /// <returns>
            /// If found, the <see cref="T:System.Configuration.SettingsProvider"/> that can persist the specified settings property; otherwise, null.
            /// </returns>
            SettingsProvider ISettingsProviderService.GetSettingsProvider(SettingsProperty property)
            {
                return this;
            }

            #endregion

            #region Private methods

            /// <summary>
            /// Determines whether the specified settings is in user scope.
            /// </summary>
            /// <param name="setting">The setting.</param>
            /// <returns>
            /// 	<c>true</c> if the specified settings is in user scope; otherwise, <c>false</c>.
            /// </returns>
            bool IsUserSetting(SettingsProperty setting)
            {
                bool userScope = setting.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;

                bool applicationScope = setting.Attributes[typeof(ApplicationScopedSettingAttribute)] is ApplicationScopedSettingAttribute;

                if (userScope && applicationScope)
                {
                    throw new ConfigurationErrorsException(string.Format("Both application and user scope is specified for setting '{0}'.", setting.Name));
                }

                if (!userScope && !applicationScope)
                {
                    throw new ConfigurationErrorsException(string.Format("No scope is specified for setting '{0}'.", setting.Name));
                }

                return userScope;
            }

            #endregion
        }

        #endregion
    }
}
