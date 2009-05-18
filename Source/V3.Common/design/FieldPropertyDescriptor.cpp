#include "stdafx.h"


#include "FieldPropertyDescriptor.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::ComponentModel::Design::Serialization;
using namespace System::Reflection;

namespace V3
{
namespace Design
{
	FieldPropertyDescriptor::FieldPropertyDescriptor( FieldInfo^ fieldInfo )
		: PropertyDescriptor( fieldInfo->Name, 
		static_cast<array<Attribute^>^>( fieldInfo->GetCustomAttributes(true) ) )
	{
		m_FieldInfo = fieldInfo;
	}

	Type^ FieldPropertyDescriptor::ComponentType::get()
	{
		return m_FieldInfo->DeclaringType;
	}

	bool FieldPropertyDescriptor::IsReadOnly::get()
	{
		return false;
	}

	Type^ FieldPropertyDescriptor::PropertyType::get()
	{
		return m_FieldInfo->FieldType;
	}

	bool FieldPropertyDescriptor::CanResetValue( Object^ )
	{
		return false;
	}

	void FieldPropertyDescriptor::ResetValue( Object^ )
	{
		//don't need to do anything
	}

	Object^ FieldPropertyDescriptor::GetValue( Object^ component )
	{
		return m_FieldInfo->GetValue( component );
	}

	void FieldPropertyDescriptor::SetValue( Object^ component, Object^ value )
	{
		m_FieldInfo->SetValue( component, value );
	}

	bool FieldPropertyDescriptor::ShouldSerializeValue( Object^ )
	{
		return true;
	}
}
}