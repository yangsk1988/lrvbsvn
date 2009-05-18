#include "stdafx.h"

#include <windows.h>
#include <dxerr.h>

#include "Configuration.h"
#include "DXException.h"

using namespace System;
using namespace System::Runtime::Serialization;

namespace V3
{
	DXException::DXException( SerializationInfo^ info, StreamingContext context )
	: Exception( info, context )
	{
		m_Result = safe_cast<Result>( info->GetValue( "Result", Result::typeid ) );
	}

	DXException::DXException()
	: Exception( "A DirectX exception occurred." )
	{
		m_Result = Result( E_FAIL );
	}

	DXException::DXException( String^ message )
	: Exception( message )
	{
		m_Result = Result( E_FAIL );
	}

	DXException::DXException( String^ message, Exception^ innerException )
	: Exception( message, innerException )
	{
		m_Result = Result( E_FAIL );
	}
	
	DXException::DXException( Result result )
	: Exception( result.ToString() )
	{
		m_Result = result;
	}
	
	Result DXException::ResultCode::get()
	{
		return m_Result;
	}
	
	void DXException::GetObjectData(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context)
	{
		if( info == nullptr )
			throw gcnew System::ArgumentNullException( "info" );

		info->AddValue("Result", m_Result);
		System::Exception::GetObjectData( info, context );
	}
}
