#include "stdafx.h"


#include "Color3.h"

using namespace System;

namespace V3
{
	Color3::Color3( float red, float green, float blue )
	: Red( red ), Green( green ), Blue( blue )
	{
	}

	bool Color3::operator == ( Color3 left, Color3 right )
	{
		return Color3::Equals( left, right );
	}

	bool Color3::operator != ( Color3 left, Color3 right )
	{
		return !Color3::Equals( left, right );
	}

	int Color3::GetHashCode()
	{
		return Red.GetHashCode() + Green.GetHashCode() + Blue.GetHashCode();
	}

	bool Color3::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Color3>( value ) );
	}

	bool Color3::Equals( Color3 value )
	{
		return ( Red == value.Red && Green == value.Green && Blue == value.Blue );
	}

	bool Color3::Equals( Color3% value1, Color3% value2 )
	{
		return ( value1.Red == value2.Red && value1.Green == value2.Green && value1.Blue == value2.Blue );
	}
}