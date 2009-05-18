#include "stdafx.h"

#include <d3dx9.h>

#include "../InternalHelpers.h"
#include "../math/Half.h"
#include "../math/Half2.h"

#include "Half2Converter.h"
#include "FieldPropertyDescriptor.h"

using namespace System;
using namespace System::Collections;
using namespace System::Drawing;
using namespace System::ComponentModel;
using namespace System::ComponentModel::Design::Serialization;
using namespace System::Globalization;
using namespace System::Reflection;

namespace V3
{
namespace Design
{
	Half2Converter::Half2Converter()
	{
		Type^ type = Half2::typeid;
		array<PropertyDescriptor^>^ propArray =
		{
			gcnew FieldPropertyDescriptor(type->GetField("X")),
			gcnew FieldPropertyDescriptor(type->GetField("Y")),
		};

		m_Properties = gcnew PropertyDescriptorCollection(propArray);
	}

	bool Half2Converter::CanConvertTo(ITypeDescriptorContext^ context, Type^ destinationType)
	{
		if( destinationType == String::typeid || destinationType == InstanceDescriptor::typeid )
			return true;
		else
			return ExpandableObjectConverter::CanConvertTo(context, destinationType);
	}

	bool Half2Converter::CanConvertFrom(ITypeDescriptorContext^ context, Type^ sourceType)
	{
		if( sourceType == String::typeid )
			return true;
		else
			return ExpandableObjectConverter::CanConvertFrom(context, sourceType);
	}

	Object^ Half2Converter::ConvertTo(ITypeDescriptorContext^ context, CultureInfo^ culture, Object^ value, Type^ destinationType)
	{
		if( destinationType == nullptr )
			throw gcnew ArgumentNullException( "destinationType" );

		if( culture == nullptr )
			culture = CultureInfo::CurrentCulture;

		Half2^ half = dynamic_cast<Half2^>( value );

		if( destinationType == String::typeid && half != nullptr )
		{
			String^ separator = culture->TextInfo->ListSeparator + " ";
			TypeConverter^ converter = TypeDescriptor::GetConverter(Half::typeid);
			array<String^>^ stringArray = gcnew array<String^>( 2 );

			stringArray[0] = converter->ConvertToString( context, culture, half->X );
			stringArray[1] = converter->ConvertToString( context, culture, half->Y );

			return String::Join( separator, stringArray );
		}
		else if( destinationType == InstanceDescriptor::typeid && half != nullptr )
		{
			ConstructorInfo^ info = (Half2::typeid)->GetConstructor( gcnew array<Type^> { Half::typeid, Half::typeid } );
			if( info != nullptr )
				return gcnew InstanceDescriptor( info, gcnew array<Object^> { half->X, half->Y } );
		}

		return ExpandableObjectConverter::ConvertTo(context, culture, value, destinationType);
	}

	Object^ Half2Converter::ConvertFrom(ITypeDescriptorContext^ context, CultureInfo^ culture, Object^ value)
	{
		if( culture == nullptr )
			culture = CultureInfo::CurrentCulture;

		String^ string = dynamic_cast<String^>( value );

		if( string != nullptr )
		{
			string = string->Trim();
			TypeConverter^ converter = TypeDescriptor::GetConverter(Half::typeid);
			array<String^>^ stringArray = string->Split( culture->TextInfo->ListSeparator[0] );

			if( stringArray->Length != 2 )
				throw gcnew ArgumentException("Invalid half format.");

			Half X = safe_cast<Half>( converter->ConvertFromString( context, culture, stringArray[0] ) );
			Half Y = safe_cast<Half>( converter->ConvertFromString( context, culture, stringArray[1] ) );

			return gcnew Half2(X, Y);
		}

		return ExpandableObjectConverter::ConvertFrom(context, culture, value);
	}

	bool Half2Converter::GetCreateInstanceSupported(ITypeDescriptorContext^ context)
	{
		DX_UNREFERENCED_PARAMETER(context);

		return true;
	}

	Object^ Half2Converter::CreateInstance(ITypeDescriptorContext^ context, IDictionary^ propertyValues)
	{
		DX_UNREFERENCED_PARAMETER(context);

		if( propertyValues == nullptr )
			throw gcnew ArgumentNullException( "propertyValues" );

		return gcnew Half2( safe_cast<Half>( propertyValues["X"] ), safe_cast<Half>( propertyValues["Y"] ) );
	}

	bool Half2Converter::GetPropertiesSupported(ITypeDescriptorContext^)
	{
		return true;
	}

	PropertyDescriptorCollection^ Half2Converter::GetProperties(ITypeDescriptorContext^, Object^, array<Attribute^>^)
	{
		return m_Properties;
	}
}
}