#include "StdAfx.h"
#include "Collection.h"
#include "Physics.h"
#include "Scene.h"
//#include <PxExtensionsAPI.h>
//#include <PxSerialFramework.h> 

Collection::Collection(PxCollection* collection, PhysX::IDisposable^ owner)
{
	if (collection == NULL)
		throw gcnew ArgumentNullException("collection");
	ThrowIfNullOrDisposed(owner, "owner");

	_collection = collection;

	ObjectTable::Add((intptr_t)collection, this, owner);
}
Collection::~Collection()
{
	this->!Collection();
}
Collection::!Collection()
{
	OnDisposing(this, nullptr);

	if (Disposed)
		return;

	this->UnmanagedPointer->release();
	_collection = NULL;

	OnDisposed(this, nullptr);
}

bool Collection::Disposed::get()
{
	return (_collection == NULL);
}

void Collection::Add(IPhysXEntity^ object)
{
	PxBase* unmanaged = (PxBase*)ObjectTable::GetObject(object);

	_collection->add(*unmanaged);
}
void Collection::AddId(IPhysXEntity^ object, long reference)
{
	PxBase* unmanaged = (PxBase*)ObjectTable::GetObject(object);

	_collection->addId(*unmanaged, reference);
}
void Collection::AddRequired(IPhysXEntity^ object)
{
	PxBase* unmanaged = (PxBase*)ObjectTable::GetObject(object);

	_collection->addRequired(*unmanaged);
}

bool Collection::Contains(IPhysXEntity^ object)
{
	PxBase* unmanaged = (PxBase*)ObjectTable::GetObject(object);

	return _collection->contains(*unmanaged);
}

IPhysXEntity^ Collection::GetObject(int index)
{
	throw gcnew NotImplementedException();
	//if (index < 0 || index >= _collection->getNbObjects())
	//	throw gcnew ArgumentOutOfRangeException("index");

	//PxBase obj = _collection->getObject(index);

	//ObjectTable::GetObject((intptr_t)(&obj));
}

void Collection::Remove(IPhysXEntity^ object)
{
	PxBase* unmanaged = (PxBase*)ObjectTable::GetObject(object);

	_collection->remove(*unmanaged);
}
void Collection::RemoveId(long reference)
{
	_collection->removeId(reference);
}

int Collection::NumberOfObjects::get()
{
	return _collection->getNbObjects();
}

int Collection::NumberOfIds::get()
{
	return _collection->getNbIds();
}

//

PxCollection* Collection::UnmanagedPointer::get()
{
	return _collection;
}