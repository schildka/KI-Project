#pragma once
#include "list"
#include "Vector2.h"

class Shape
{
public:

	std::list<Vector2> ShapeLocations;
	Shape(std::list<Vector2> ShapeLocations);
	~Shape();
	void free();
};

