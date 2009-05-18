
#pragma once

using namespace System::Reflection;
using namespace System::ComponentModel;

namespace V3
{
	namespace Design
	{
		ref class FieldPropertyDescriptor : PropertyDescriptor
		{
		private:
			FieldInfo^ m_FieldInfo;

		public:
			FieldPropertyDescriptor( FieldInfo^ fieldInfo );

			property System::Type^ ComponentType
			{
				virtual System::Type^ get() override;
			}

			property bool IsReadOnly
			{
				virtual bool get() override;
			}

			property System::Type^ PropertyType
			{
				virtual System::Type^ get() override;
			}

			virtual bool CanResetValue( System::Object^ component ) override;
			virtual void ResetValue( System::Object^ component ) override;
			virtual System::Object^ GetValue( System::Object^ component ) override;
			virtual void SetValue( System::Object^ component, System::Object^ value ) override;
			virtual bool ShouldSerializeValue( System::Object^ component ) override;
		};
	}
}
