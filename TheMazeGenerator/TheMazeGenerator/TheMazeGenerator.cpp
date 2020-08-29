// TheMazeGenerator.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include "Shape.h"
#include "list"
#include "LShapes.h"
#include "Vector2.h"
#include "cstdlib"
#include "unordered_map"
#include "algorithm"
#include <iostream>
#include "string"
#include <fstream>
#include "time.h"

enum type;
void initWorld();
void UpdateWorldWithShape(Shape &SettedShape, Vector2 &RandomLocation, std::list<Vector2>& ShapeLocations,type tiletype);
bool getElement(int row, int column);
Shape GetRandomShapeElement(std::list<Shape> shapelist);
Vector2 GetRandomMapLocation();
bool CheckLocationOnValidity(std::list<Vector2>& ShapeLocations);
bool ContainsList(std::list<Vector2> yourList, Vector2 value);
void GenerateShapeLocations(Shape &SettedShape, Vector2 &RandomLocation, std::list<Vector2>& ShapeLocations);
bool ContainsWorld(Vector2 value);
bool CheckValidPaths();


struct AllShapes {
	std::list<Shape> AllShapesList = std::list<Shape>();

	AllShapes() {

		Shape S_Plus = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(1,0),Vector2(0,1),Vector2(0,-1) });

		Shape Cube_Shape = Shape({ Vector2(0,0),Vector2(0,1),Vector2(1,0),Vector2(1,1) });

		Shape LRightSmall = Shape({ Vector2(0,0),Vector2(1,0),Vector2(0,1) });
		Shape LLeftSmall = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1) });
		Shape L90RightSmall = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0) });
		Shape L180ReflectedSmall = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(-1,0) });

		Shape LRight = Shape({ Vector2(0,0),Vector2(1,0),Vector2(0,1),Vector2(0,2) });
		Shape LLeft = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1),Vector2(0,2) });
		Shape L90Right = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(1,0),Vector2(2,0) });
		Shape L180Reflected = Shape({ Vector2(0,0),Vector2(0,-1),Vector2(-1,0),Vector2(-2,0) });

		Shape I_MidVertical = Shape({ Vector2(0,0),Vector2(0,1),Vector2(0,2) });
		Shape I_MidHorizontal = Shape({ Vector2(0,0),Vector2(1,0),Vector2(2,0) });

		Shape I_SmallVertical = Shape({ Vector2(0,0),Vector2(0,1) });
		Shape I_SmallHorizontal = Shape({ Vector2(0,0),Vector2(1,0) });

		Shape T_SmallLeft = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1),Vector2(0,-1) });
		Shape T_SmallRight = Shape({ Vector2(0,0),Vector2(1,0),Vector2(0,1),Vector2(0,-1) });
		Shape T_SmallTop = Shape({ Vector2(0,0),Vector2(1,0),Vector2(-1,0),Vector2(0,1) });
		Shape T_SmallDown = Shape({ Vector2(0,0),Vector2(1,0),Vector2(-1,0),Vector2(0,-1) });

		Shape T_BigLeft = Shape({ Vector2(0,0),Vector2(-1,0),Vector2(0,1),Vector2(0,-1),Vector2(-2,0) });
		Shape T_BigRight = Shape({ Vector2(0,0),Vector2(1,0),Vector2(0,1),Vector2(0,-1),Vector2(2,0) });
		Shape T_BigTop = Shape({ Vector2(0,0),Vector2(1,0),Vector2(-1,0),Vector2(0,1),Vector2(0,2) });
		Shape T_BigDown = Shape({ Vector2(0,0),Vector2(1,0),Vector2(-1,0),Vector2(0,-1),Vector2(0,-2) });

		Shape BOX = Shape({Vector2(0,0)});

		this->AllShapesList.push_front(S_Plus);
		this->AllShapesList.push_front(Cube_Shape);
		this->AllShapesList.push_front(LRightSmall);
		this->AllShapesList.push_front(LLeftSmall);
		this->AllShapesList.push_front(L90RightSmall);
		this->AllShapesList.push_front(L180ReflectedSmall);
		this->AllShapesList.push_front(LRight);
		this->AllShapesList.push_front(LLeft);
		this->AllShapesList.push_front(L90Right);
		this->AllShapesList.push_front(L180Reflected);
		this->AllShapesList.push_front(I_MidVertical);
		this->AllShapesList.push_front(I_MidHorizontal);
		this->AllShapesList.push_front(I_SmallVertical);
		this->AllShapesList.push_front(I_SmallHorizontal);
		this->AllShapesList.push_front(T_SmallLeft);
		this->AllShapesList.push_front(T_SmallRight);
		this->AllShapesList.push_front(T_SmallTop);
		this->AllShapesList.push_front(T_SmallDown);
		this->AllShapesList.push_front(T_BigLeft);
		this->AllShapesList.push_front(T_BigRight);
		this->AllShapesList.push_front(T_BigTop);
		this->AllShapesList.push_front(T_BigDown);
	};



};

int main()
{
	std::cout << "Hello World!\n";
	initWorld();
	int bla;
	std::cin >> bla;

}

const int xLength = 6;
const int yLength = 9;
bool world[xLength * yLength];

enum type {IsClaimed = 0, IsFree = 1, IsFood = 2, IsPallet = 3, IsWall = 4};

struct GoodTile {
	int tiletype = type::IsFree;
	Vector2 ShapeLocation = Vector2(0,0);
	Shape ShapeType = Shape({});
	char counter = 0;

	GoodTile() {};
};




struct hash_fn {
	std::size_t operator()(const Vector2& vec) const {
		std::size_t h1 = std::hash<int>()(vec.x + vec.y);
		return h1 ^ h1;
	}
};

std::unordered_map<Vector2, GoodTile, hash_fn> WorldWithVectors;
char counter = 'a';

void initWorld() {

	//28 x31
	std::ofstream myfile;
	myfile.open("maze.txt");
	
	

	for (bool& val : world)val = true;
	for (int i = 0; i < xLength*yLength; ++i)
	{
		int xVal = i % xLength;
		int yVal = i / xLength;
		WorldWithVectors.insert(std::pair<Vector2, GoodTile>(Vector2(xVal, yVal), GoodTile()));
		//WorldWithVectors[Vector2(xVal, yVal)] = Tile();
	}

	std::list<Shape> AllShapes2 = std::list<Shape>();

	//Init All Shapes
	LShapes l_shapes = LShapes();

	AllShapes shapes_list = AllShapes();

	Shape StartSpawnGhosts = Shape({Vector2(0,0),Vector2(0,1),Vector2(1,0),Vector2(1,1)});
	//Shape Claimed = Shape({ Vector2(0,1),Vector2(0,0),Vector2(1,0),Vector2(2,0),Vector2(1,1),Vector2(2,1),Vector2(0,-1),Vector2(1,-1),Vector2(2,-1)});
	std::list<Vector2> StartSpawn = std::list<Vector2>();
	Vector2 locationSpawn(Vector2(0, (int)(yLength / 2.0f)-1));


	UpdateWorldWithShape(StartSpawnGhosts, locationSpawn, StartSpawn,type::IsWall);
	try {
		srand(time(0));
	}
	catch (...) {};
	
	
	
	int Index = rand() % (yLength);
	for (int x = 0; x<xLength;++x )
	{
		for (int y = 0; y <yLength;++y)
		{
			int Rand_Num = (Index + y) % yLength;
			Vector2 current = Vector2(x, y);
			if (WorldWithVectors[current].tiletype == type::IsWall)continue;
			Shape UsedShape = Shape({});
			bool isValidShape = true;
			do 
			{
				UsedShape = GetRandomShapeElement(shapes_list.AllShapesList);
				isValidShape = true;
				for (auto& val : UsedShape.ShapeLocations) {
					if (ContainsWorld(val + current) && WorldWithVectors[val + current].tiletype == type::IsWall)isValidShape = false;
				};
			} while (!isValidShape);
			
			std::list<Vector2> locs;
			UpdateWorldWithShape(UsedShape, current, locs, type::IsWall);
		}
	}
	
	std::list<Vector2> punkte = {Vector2(0,11),Vector2(0,12),Vector2(0,10)};

	std::string printstring = ""; 
	
	for (int x = 0; x < 28; ++x) {
		printf("#");
		myfile << "#";
	}
	printf("\n");

	myfile << "\n";
	
	for (int y = 0; y < yLength*3; ++y)
	{
		for (int x = 0; x < xLength; ++x)
		{
			
			
			Vector2 current = Vector2(x , y/3);
			Vector2 LocallocInShape = WorldWithVectors[current].ShapeLocation;
			Shape BinShape = WorldWithVectors[current].ShapeType;
			int state = y % 3;
			if (x == 0 && y!=0) {
				std::string Reverse = printstring;
				std::string begin = "#";
				std::reverse(Reverse.begin(), Reverse.end());
				if (y == 12) {
					Reverse = Reverse.substr(0, Reverse.size() - 1);
					Reverse.append("l");
				}
				if (y == 9) {
					Reverse = Reverse.substr(0, Reverse.size() - 1);
					Reverse.append("g");
				}
				if (y == 15) {
					Reverse = Reverse.substr(0, Reverse.size() - 1);
					Reverse.append("p");
				}
				Reverse.append(printstring);
				begin.append(Reverse);
				begin.append("#");
				
				printf(("%s"), begin.c_str());

				myfile << begin.c_str();

				printstring = "";
				printf("\n");

				myfile << "\n";
			}
			

			Vector2 up(0, -1);
			Vector2 down(0, 1);
			Vector2 right(1, 0);
			Vector2 left(-1, 0);

			/*
			bool isUp = ContainsList(BinShape.ShapeLocations, up + LocallocInShape);
			if (!ContainsWorld(up + LocallocInShape))isUp = false;

			bool isLeft = ContainsList(BinShape.ShapeLocations, left + LocallocInShape);
			if (!ContainsWorld(left + LocallocInShape))isLeft = false;

			bool isUpLeft = ContainsList(BinShape.ShapeLocations, left + up + LocallocInShape);
			if (!ContainsWorld(up + left +LocallocInShape))isUpLeft = false;

			bool isUpRight = ContainsList(BinShape.ShapeLocations, right + up + LocallocInShape);
			if (!ContainsWorld(up + right+ LocallocInShape))isUpRight = false;

			bool isRight = ContainsList(BinShape.ShapeLocations, right + LocallocInShape);
			if (!ContainsWorld(right + LocallocInShape))isRight = false;

			bool isDown = ContainsList(BinShape.ShapeLocations, down + LocallocInShape);
			if (!ContainsWorld(down + LocallocInShape))isDown = false;

			bool isDownLeft = ContainsList(BinShape.ShapeLocations, down +left+ LocallocInShape);
			if (!ContainsWorld(down +left + LocallocInShape))isDownLeft = false;

			bool isDownRight = ContainsList(BinShape.ShapeLocations, down + right + LocallocInShape);
			if (!ContainsWorld(down+right + LocallocInShape))isDownRight = false;
			*/
			//


			
			
			bool isUp = ContainsList(BinShape.ShapeLocations, up + LocallocInShape);
			if (!ContainsWorld(up + current))isUp = false;

			bool isLeft = ContainsList(BinShape.ShapeLocations, left + LocallocInShape);
			if (!ContainsWorld(left + current))isLeft = false;

			bool isUpLeft = ContainsList(BinShape.ShapeLocations, left + up + LocallocInShape);
			if (!ContainsWorld(up + left + current))isUpLeft = false;

			bool isUpRight = ContainsList(BinShape.ShapeLocations, right + up + LocallocInShape);
			if (!ContainsWorld(up + right + current))isUpRight = false;

			bool isRight = ContainsList(BinShape.ShapeLocations, right + LocallocInShape);
			if (!ContainsWorld(right + current))isRight = false;

			bool isDown = ContainsList(BinShape.ShapeLocations, down + LocallocInShape);
			if (!ContainsWorld(down + current))isDown = false;

			bool isDownLeft = ContainsList(BinShape.ShapeLocations, down + left + LocallocInShape);
			if (!ContainsWorld(down + left + current))isDownLeft = false;

			bool isDownRight = ContainsList(BinShape.ShapeLocations, down + right + LocallocInShape);
			if (!ContainsWorld(down + right + current))isDownRight = false;
			
			if (WorldWithVectors[current].tiletype != type::IsWall)continue;
			
			
			if (y == 0){

				if (isUp && !isLeft && !isRight) {
					if (x == 0)printstring.append(".#.");
					else printstring.append("#.");
					continue;
				}
				if (isUp && isLeft && isRight && isUpRight && isUpLeft) {
					if (x == 0)printstring.append("###");
					else printstring.append("##");
					continue;
				}
				if (!isUp) {
					if (x == 0)printstring.append("...");
					else printstring.append("..");
					continue;
				}
				if (isUp && isLeft && isUpLeft && (!isUpRight || !isRight)) {
					if (x == 0)printstring.append("##.");
					else printstring.append("#.");
					continue;
				}
				if (isUp && isRight && isUpRight && (!isUpLeft || !isLeft)) {
					if (x == 0)printstring.append(".##");
					else printstring.append("##");
					continue;
				}

				if (isUp && (!isLeft || !isUpLeft) && (!isUpRight || !isRight)) {
					if (x == 0)printstring.append(".#.");
					else printstring.append("#.");
					continue;
				}
				if (!isUp && isRight && isUpRight && (!isUpLeft || !isLeft)) {
					if (x == 0)printstring.append("..#");
					else printstring.append(".#");
					continue;
				}

			}
			/*
			printf("hello 0");
			if (isUp)printf("IsUpTrue  ");
			else printf("IsUpFalse  ");

			if (isLeft)printf("IsLeftTrue  ");
			else printf("IsLeftFalse  ");

			if (isRight)printf("IsRightTrue  ");
			else printf("IsRightFalse  ");

			if (isDown)printf("IsDownTrue  ");
			else printf("IsDownFalse  ");



			//

			if (isUpLeft)printf("IsUpLeftTrue  ");
			else printf("IsUpLeftFalse  ");

			if (isUpRight)printf("IsUpRightTrue  ");
			else printf("IsUpRightFalse  ");

			if (isDownLeft)printf("IsDownLeftTrue  ");
			else printf("IsDownLeftFalse  ");

			if (isDownRight)printf("IsDownRightTrue  ");
			else printf("IsDownRightFalse  ");

			printf("\n");

			//printstring.append("hello 0");
			*/

			switch (state)
			{

			case 0:
				
			case 1:
				if (ContainsList(punkte, Vector2(x, y))) {
					printstring.append("...");
					continue;
				}


				if (x == 0) {
					printstring.append("###");
					continue;
				}

				if (isLeft && isRight) {
					if (x == 0)printstring.append("###");
					else printstring.append("##");
					continue;
				}
				if (!isLeft && isRight) {
					if (x == 0)printstring.append(".##");
					else printstring.append("##");
					continue;
				}
				if (!isRight && isLeft) {
					if (x == 0)printstring.append("##.");
					else printstring.append("#.");
					continue;
				}
				if (!isRight && !isLeft) {
					if (x == 0)printstring.append("...");// .#.
					else printstring.append("#.");
					continue;
				}
				/*
				printf("hello 1");
				if(isUp)printf("IsUpTrue  ");
				else printf("IsUpFalse  ");

				if (isLeft)printf("IsLeftTrue  ");
				else printf("IsLeftFalse  ");

				if (isRight)printf("IsRightTrue  ");
				else printf("IsRightFalse  ");

				if (isDown)printf("IsDownTrue  ");
				else printf("IsDownFalse  ");

				//

				if (isUpLeft)printf("IsUpLeftTrue  ");
				else printf("IsUpLeftFalse  ");

				if (isUpRight)printf("IsUpRightTrue  ");
				else printf("IsUpRightFalse  ");

				if (isDownLeft)printf("IsDownLeftTrue  ");
				else printf("IsDownLeftFalse  ");

				if (isDownRight)printf("IsDownRightTrue  ");
				else printf("IsDownRightFalse  ");

				printf("\n");
				
					3###
					2 ##.
					#.# //impossible
					3 .##
					...
					..#
					#..
					1.#.

				*/
				 
				break;
			case 2:
				if (ContainsList(punkte, Vector2(x, y))) {
					printstring.append("...");
					continue;
				}
				
				if (isDown && !isLeft && !isRight) {
					if (x == 0)printstring.append("...");//.#.
					else printstring.append("#.");
					continue;
				}
				if (isDown && isLeft && isRight && isDownRight && isDownLeft) {
					if (x == 0)printstring.append("###");
					else printstring.append("##");
					continue;
				}
				if (!isDown) {
					if (x == 0)printstring.append("...");
					else printstring.append("..");
					continue;
				}
				if (isDown && isLeft && isDownLeft && (!isDownRight || !isRight)) {
					if (x == 0)printstring.append("##.");
					else printstring.append("#.");
					continue;
				}
				if (isDown && isRight && isDownRight && (!isDownLeft || !isLeft)) {
					if (x == 0)printstring.append(".##");
					else printstring.append("##");
					continue;
				}

				if (!isDown && isRight && isDownRight && (!isDownLeft || !isLeft)) {
					if (x == 0)printstring.append("..#");
					else printstring.append(".#");
					continue;
				}

				if (isDown && (!isLeft || !isDownLeft) && (!isDownRight || !isRight)) {
					if (x == 0)printstring.append("..."); //.#.
					else printstring.append("#.");
					continue;
				}



			default:
				
				break;
			}



			
		}
		
	}


	for (int x = 0; x < 28; ++x) {
		if (x == 0 || x == 27) {
			printf("#");
			myfile << "#";
		}
		else {
			printf(".");
			myfile << ".";
		}
	}
	printf("\n");
	myfile << "\n";
	for (int x = 0; x < 28; ++x) {
		printf("#");
		myfile << "#";
	}
	myfile.close();

	printf("\n\n");
	
	std::string line = "";
	for (int i = 0; i < xLength*yLength; ++i)
	{
		int xVal = i % xLength;
		int yVal = i / xLength;
		
		if (xVal == 0) {
			std::string enil = line;
			std::reverse(enil.begin(), enil.end());
			enil.append(line);
			printf(("%s \n"), enil.c_str());
			line = "";
		}
		if (WorldWithVectors[Vector2(xVal, yVal)].tiletype == type::IsFree ||
			WorldWithVectors[Vector2(xVal, yVal)].tiletype == type::IsClaimed) {
			line.append(".");
		}
		else {
			std::string helloo(1,WorldWithVectors[Vector2(xVal, yVal)].counter);//std::to_string(WorldWithVectors[Vector2(xVal, yVal)].counter);
			line.append(helloo);
			line.append(" ");
		}
	}
}
bool CheckValidPaths() {
	Vector2 right(1, 0);
	Vector2 left(-1, 0);
	Vector2 top(0, 1);
	Vector2 bottom(0, -1);

	for (int i = 0; i < xLength*yLength; ++i) {

		Vector2 curr = Vector2(i % xLength, i / xLength);
		int counter = 0;
		if (WorldWithVectors[curr].tiletype != type::IsFree)continue;
		if (ContainsWorld(curr + right) && WorldWithVectors[curr + right].tiletype == type::IsFree)++counter;
		if (ContainsWorld(curr + left) && WorldWithVectors[curr + left].tiletype == type::IsFree)++counter;
		if (ContainsWorld(curr + top) && WorldWithVectors[curr + top].tiletype == type::IsFree)++counter;
		if (ContainsWorld(curr + bottom) && WorldWithVectors[curr + bottom].tiletype == type::IsFree)++counter;
		if (counter < 2)return false;
	}
	return true;

}
bool CheckLocationOnValidity(std::list<Vector2>& ShapeLocations) {
	Vector2 right(1, 0);
	Vector2 left(-1, 0);
	Vector2 top(0, 1);
	Vector2 bottom(0, -1);
	try 
	{
;
		for (auto& loc : ShapeLocations) {
			if (WorldWithVectors.at(loc).tiletype != type::IsFree)return false;

			
			if (ContainsList(ShapeLocations, loc + right)) return false;
			if (!ContainsWorld(loc+ right) && WorldWithVectors.at(loc + right).tiletype != type::IsFree)return false;
			if (!ContainsWorld(loc + right+ right+right) && WorldWithVectors.at(loc + right + right).tiletype == type::IsFree && WorldWithVectors.at(loc + right + right + right).tiletype != type::IsFree)return false;
			
			if (ContainsList(ShapeLocations, loc + left))return false;
			if (!ContainsWorld(loc + left) && !WorldWithVectors.at(loc + left).tiletype != type::IsFree)return false;
			if (!ContainsWorld(loc + left + left + left) && WorldWithVectors.at(loc + left + left).tiletype == type::IsFree && !WorldWithVectors.at(loc + left + left + left).tiletype != type::IsFree)return false;
			
			if (ContainsList(ShapeLocations, loc + top))return false;
			if (!ContainsWorld(loc + top) && !WorldWithVectors.at(loc + top).tiletype != type::IsFree)return false;
			if (!ContainsWorld(loc + top + top + top) && WorldWithVectors.at(loc + top + top).tiletype == type::IsFree && !WorldWithVectors.at(loc + top + top + top).tiletype != type::IsFree)return false;
			
			if (ContainsList(ShapeLocations, loc + bottom)) return false;
			if (!ContainsWorld(loc + bottom) && !WorldWithVectors.at(loc + bottom).tiletype != type::IsFree)return false;
			if (!ContainsWorld(loc + bottom + bottom + bottom) && WorldWithVectors.at(loc + bottom + bottom).tiletype == type::IsFree && !WorldWithVectors.at(loc + bottom + bottom + bottom).tiletype != type::IsFree)return false;
		
		}
	}
	catch (...) {};
	return true;
}

bool ContainsWorld(Vector2 value) {

	return (WorldWithVectors.find(value) != WorldWithVectors.end());
}


bool ContainsList(std::list<Vector2> yourList, Vector2 value) {

	return std::find(yourList.begin(), yourList.end(), value) != yourList.end();
}

void UpdateWorldWithShape(Shape &SettedShape, Vector2 &RandomLocation, std::list<Vector2>& ShapeLocations,type tiletype)
{
	for (Vector2& vec : SettedShape.ShapeLocations) {
		Vector2 newLocation = vec + RandomLocation;
		if (ContainsWorld(newLocation)) {
			ShapeLocations.push_front(newLocation);
			WorldWithVectors[newLocation].tiletype = tiletype;
			WorldWithVectors[newLocation].ShapeType = SettedShape;
			WorldWithVectors[newLocation].ShapeLocation = vec;
			WorldWithVectors[newLocation].counter = counter;
		}
	}
	counter++;
}

void GenerateShapeLocations(Shape &SettedShape, Vector2 &RandomLocation, std::list<Vector2>& ShapeLocations) {
	
	for (Vector2& vec : SettedShape.ShapeLocations) {
		Vector2 newLocation = vec + RandomLocation;
		if (WorldWithVectors.find(newLocation) != WorldWithVectors.end()) {
			ShapeLocations.push_front(newLocation);
		}
	}
}

Shape GetRandomShapeElement(std::list<Shape> shapelist) {
	int randomNumber = rand() % shapelist.size();
	Shape SettedShape = *std::next(shapelist.begin(), randomNumber);
	return SettedShape;
}

bool getElement(int row, int column) {
	if (row*column >= xLength * yLength) {
		std::cout << "Index Out of Bounds !!!!!";
		return false;
	}
	return world[row*column];
}

Vector2 GetRandomMapLocation() {
	int Index = rand() % (xLength*yLength);
	int xVal = Index % xLength;
	int yVal = Index / xLength;
	return Vector2(xVal, yVal);

}


// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
