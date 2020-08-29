#include "pch.h"
#include "Vector2.h"
#include "LShapes.h"
#include <iostream>


void LShapes::free()
{
	for (auto& shape :LShapesList) {
		shape.free();
	}
	LShapesList.empty();
	delete this;
}

std::list<Shape> LShapes::GetAllShapes()
{
	return LShapesList;
}

LShapes::LShapes()
{
	this->LShapesList = std::list<Shape>();

	//L 2 Up 1 Left
	Shape LRight = Shape({Vector2(0,0),Vector2(1,0),Vector2(0,1),Vector2(0,2)});
	LShapesList.push_front(LRight);
	Shape LLeft = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1),Vector2(0,2) });
	LShapesList.push_front(LLeft);
	Shape L90Right = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0),Vector2(2,0) });
	LShapesList.push_front(L90Right);
	Shape L90Left = Shape({ Vector2(0,0),Vector2(0,1),Vector2(-1,0),Vector2(-2,0) });
	LShapesList.push_front(L90Left);
	Shape L180Right = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,-1),Vector2(0,-2) });
	LShapesList.push_front(L180Right);
	Shape L180Reflected = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0),Vector2(2,0) });
	LShapesList.push_front(L90Right);
	
	//L 1 Up 1 Left
	Shape LRightSmall = Shape({ Vector2(0,0),Vector2(1,0),Vector2(0,1) });
	LShapesList.push_front(LRightSmall);
	Shape LLeftSmall = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1)});
	LShapesList.push_front(LLeftSmall);
	Shape L90RightSmall = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0)});
	LShapesList.push_front(L90RightSmall);
	Shape L90LeftSmall = Shape({ Vector2(0,0),Vector2(0,1),Vector2(-1,0) });
	LShapesList.push_front(L90LeftSmall);
	Shape L180RightSmall = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,-1) });
	LShapesList.push_front(L180RightSmall);
	Shape L180ReflectedSmall = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0) });
	LShapesList.push_front(L90RightSmall);

	//L 2 UP 2 Left Symmetrie2
	Shape LRightMedium = Shape({ Vector2(0,0),Vector2(1,0),Vector2(2,0),Vector2(0,1),Vector2(0,2) });
	LShapesList.push_front(LRightMedium);
	Shape LLeftMedium = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(-2,0),Vector2(0,1),Vector2(0,2) });
	LShapesList.push_front(LLeftMedium);
	Shape L90RightMedium = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(0,-2),Vector2(1,0),Vector2(2,0) });
	LShapesList.push_front(L90RightMedium);
	Shape L90LeftMedium = Shape({ Vector2(0,0),Vector2(0,1),Vector2(0,2),Vector2(-1,0),Vector2(-2,0) });
	LShapesList.push_front(L90LeftMedium);
	Shape L180RightMedium = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(-2,0),Vector2(0,-1),Vector2(0,-2) });
	LShapesList.push_front(L180RightMedium);
	
	//L 3 Up 2 Left
	Shape LRightBig = Shape({ Vector2(0,0),Vector2(1,0),Vector2(2,0),Vector2(0,1),Vector2(0,2),Vector2(0,3) });
	LShapesList.push_front(LRightBig);
	Shape LLeftBig = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(-2,0),Vector2(0,1),Vector2(0,2),Vector2(0,3) });
	LShapesList.push_front(LLeftBig);
	Shape L90RightBig = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(0,-2),Vector2(1,0),Vector2(2,0),Vector2(3,0) });
	LShapesList.push_front(L90RightBig);
	Shape L90LeftBig = Shape({ Vector2(0,0),Vector2(0,1),Vector2(0,2),Vector2(-1,0),Vector2(-2,0),Vector2(-3,0) });
	LShapesList.push_front(L90LeftBig);
	Shape L180RightBig = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(-2,0),Vector2(0,-1),Vector2(0,-2),Vector2(0,-3) });
	LShapesList.push_front(L180RightBig);
}



LShapes::~LShapes()
{
}
