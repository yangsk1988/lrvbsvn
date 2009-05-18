#include "stdafx.h"


#include "../Viewport.h"

#include "Half.h"

using namespace System;
using namespace System::Globalization;

namespace V3
{
	Half::Half( float value )
	{
		Half temp;
		D3DXFloat32To16Array( reinterpret_cast<D3DXFLOAT16*>( &temp ), &value, 1 );
		m_Value = temp.m_Value;
	}

	array<float>^ Half::ConvertToFloat( array<Half>^ values )
	{
		array<float>^ results = gcnew array<float>( values->Length );
		pin_ptr<float> pinnedResults = &results[0];
		pin_ptr<Half> pinnedValues = &values[0];

		D3DXFloat16To32Array( reinterpret_cast<FLOAT*>( pinnedResults ), 
			reinterpret_cast<const D3DXFLOAT16*>( pinnedValues ), values->Length );

		return results;
	}

	array<Half>^ Half::ConvertToHalf( array<float>^ values )
	{
		array<Half>^ results = gcnew array<Half>( values->Length );
		pin_ptr<Half> pinnedResults = &results[0];
		pin_ptr<float> pinnedValues = &values[0];

		D3DXFloat32To16Array( reinterpret_cast<D3DXFLOAT16*>( pinnedResults ), 
			reinterpret_cast<const FLOAT*>( pinnedValues ), values->Length );

		return results;
	}

	Half::operator Half( float value )
	{
		return Half( value );
	}

	Half::operator float( Half value )
	{
		float result;
		D3DXFloat16To32Array( &result, reinterpret_cast<D3DXFLOAT16*>( &value ), 1 );
		return result;
	}

	bool Half::operator == ( Half left, Half right )
	{
		return Half::Equals( left, right );
	}

	bool Half::operator != ( Half left, Half right )
	{
		return !Half::Equals( left, right );
	}

	String^ Half::ToString()
	{
		Half temp = Half( m_Value );
		return ( static_cast<float>( temp ) ).ToString( CultureInfo::CurrentCulture );
	}

	int Half::GetHashCode()
	{
		return (m_Value * 3 / 2) ^ m_Value;
	}

	bool Half::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Half>( value ) );
	}

	bool Half::Equals( Half value )
	{
		return ( value.m_Value == m_Value );
	}

	bool Half::Equals( Half% value1, Half% value2 )
	{
		return ( value1.m_Value == value2.m_Value );
	}
}