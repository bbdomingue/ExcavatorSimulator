
//==========================================================================----
//== Copyright NaturalPoint ==--
//==========================================================================----

#ifndef __CAMERALIBRARY_SERIALIZER_H__
#define __CAMERALIBRARY_SERIALIZER_H__

//== INCLUDES ======================================================================-----
#include "cameracommonglobals.h"
#include "sha1.h"

//==================================================================================-----

namespace Core
{
    const int kSerializerDefaultBlockSize = 16;  //== each successive block size will double
                                                 //== till max block size is reached.
    const int kSerializerMaxBlockSize     = 1024*1024; //== 1MB block size ==--

    class CLAPI cSerializer
    {
    public:
        cSerializer    ( long Size = Core::kSerializerDefaultBlockSize );
        ~cSerializer   ();

        virtual void            WriteData(const unsigned char *Buffer, unsigned int BufferSize);

        /// <summary>Write the full contents of the given serializer into this one.</summary>
        virtual void            WriteData( const cSerializer &data );

        /// <summary>Clear all data from serializer.</summary>
        virtual void            Clear();

        /// <summary>Compute SHA1 hash tag.</summary>
        cHashTag        HashTag();

        virtual unsigned int    ReadData (unsigned char *Buffer, unsigned int BufferSize);
        virtual void            AdvanceReadPointer( unsigned int byteCount );
        virtual unsigned int    ReadLine (unsigned char *Buffer, unsigned int BufferSize);

        virtual void            WriteInt (int Value);
        virtual int             ReadInt  ();
        virtual void            WriteLongLong( long long Value );
        virtual long long       ReadLongLong();
        virtual void            WriteLong(long Value);
        virtual long            ReadLong ();
        virtual void            WriteShort(int Value);
        virtual int             ReadShort();

        virtual void            WriteDouble(double Value);
        virtual double          ReadDouble();
        virtual void            WriteFloat(float Value);
        virtual float           ReadFloat();
        virtual void            WriteBool(bool Value);
        virtual bool            ReadBool();
        virtual bool            IsEOF    ();
        virtual long            Size     () const;
        virtual long            Remaining();

        virtual int             WriteString(const char* Buffer, int BufferSize);
        virtual int             ReadString (char* Buffer, int BufferSize);

        virtual void            WriteByte(unsigned char Byte);
        virtual unsigned char   ReadByte();

        virtual void            ResetReadPointer() const;

        virtual bool            LoadFrom( const char *filename );
        virtual bool            StreamFrom( const char *filename );

        virtual bool            SaveTo( const char *filename );

#if !defined(WIN64)
        unsigned char mImplementationSpace[56];
#else
        unsigned char mImplementationSpace[80];
#endif
    };
}

#endif
