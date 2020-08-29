#include "pch.h"
#include "Shape.h"


Shape::Shape(std::list<Vector2> ShapeLocations)
{
	this->ShapeLocations = ShapeLocations;
}


Shape::~Shape()
{
	/*
	for (Vector2& locs : ShapeLocations)
	{
		locs.free();
	}
	ShapeLocations.empty();*/
}

void Shape::free()
{
	delete this;
}
