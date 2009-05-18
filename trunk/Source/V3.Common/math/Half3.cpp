#include "stdafx.h"


#include <d3dx9.h>

#include "Half.h"
#include "Half3.h"

using namespace System;
using namespace System::Globalization;

namespace V3
{
	Half3::Half3( Half value )
	{
		X = value;
		Y = value;
		Z = value;
	}

	Half3::Half3( Half x, Half y, Half z )
	{
		X = x;
		Y = y;
		Z = z;
	}

	bool Half3::operator == ( Half3 left, Half3 right )
	{
		return Half3::Equals( left, right );
	}

	bool Half3::operator != ( Half3 left, Half3 right )
	{
		return !Half3::Equals( left, right );
	}

	int Half3::GetHashCode()
	{
		return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
	}

	bool Half3::Equals( Object^ value )
	{
		if( value == nullptr )
			return false;

		if( value->GetType() != GetType() )
			return false;

		return Equals( safe_cast<Half3>( value ) );
	}

	bool Half3::Equals( Half3 value )
	{
		return ( X == value.X && Y == value.Y && Z == value.Z );
	}

	bool Half3::Equals( Half3% value1, Half3% value2 )
	{
		return ( value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z );
	}
}