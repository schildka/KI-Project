#pragma once
#include "Shape.h"
#include "list"
class LShapes
{
public:
	std::list<Shape> LShapesList;
	void free();
	std::list<Shape> GetAllShapes();
	LShapes();
	~LShapes();
};

