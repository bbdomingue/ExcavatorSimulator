#include "StdAfx.h"
#include "Actor.h"
#include "Physics.h"
#include "Scene.h"
#include "ObservableInstance.h"
#include "Serializable.h"

using namespace PhysX;

Actor::Actor(PxActor* actor, PhysX::IDisposable^ owner)
{
	if (actor == NULL)
		throw gcnew ArgumentNullException("actor");

	_actor = actor;

	ObjectTable::Add<Actor^>((intptr_t)actor, this, owner);

	this->UnmanagedOwner = true;
}
Actor::~Actor()
{
	this->!Actor();
}
Actor::!Actor()
{
	OnDisposing(this, nullptr);

	if (this->Disposed)
		return;

	if (this->UnmanagedOwner)
		_actor->release();
	_actor = NULL;

	OnDisposed(this, nullptr);
}

bool Actor::Disposed::get()
{
	return (_actor == NULL);
}

ObservableInstance^ Actor::GetObservableInstance()
{
	return gcnew ObservableInstance(_actor);
}

Serializable^ Actor::AsSerializable()
{
	return gcnew Serializable(_actor);
}

String^ Actor::ToString()
{
	return this->Name;
}

//

PhysX::Physics^ Actor::Physics::get()
{
	return this->Scene->Physics;
}
PhysX::Scene^ Actor::Scene::get()
{
	PxScene* scene = _actor->getScene();

	return ObjectTable::GetObject<PhysX::Scene^>((intptr_t)scene);
}

ActorType Actor::Type::get()
{
	return ToManagedEnum(ActorType, _actor->getType());
}

String^ Actor::Name::get()
{
	return Util::ToManagedString(_actor->getName());
}
void Actor::Name::set(String^ value)
{
	if (_actor->getName() != NULL)
		Marshal::FreeHGlobal(IntPtr((char*)_actor->getName()));

	_actor->setName(Util::ToUnmanagedString(value));
}

Bounds3 Actor::WorldBounds::get()
{
	return Bounds3::FromUnmanaged(_actor->getWorldBounds());
}

ActorFlag Actor::Flags::get()
{
	return ToManagedEnum(ActorFlag, _actor->getActorFlags());
}
void Actor::Flags::set(ActorFlag value)
{
	_actor->setActorFlags((PxActorFlags)(PxU32)value);
}

int Actor::DominanceGroup::get()
{
	return (int)_actor->getDominanceGroup();
}
void Actor::DominanceGroup::set(int value)
{
	_actor->setDominanceGroup((PxDominanceGroup)value);
}

int Actor::OwnerClient::get()
{
	return (int)_actor->getOwnerClient();
}
void Actor::OwnerClient::set(int value)
{
	_actor->setOwnerClient((PxClientID)value);
}

int Actor::ClientBehaviorBits::get()
{
	return (int)_actor->getClientBehaviorBits();
}
void Actor::ClientBehaviorBits::set(int value)
{
	_actor->setClientBehaviorBits((PxU32)value);
}

PxActor* Actor::UnmanagedPointer::get()
{
	return _actor;
}