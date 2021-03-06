// MazeGenerator.cpp : Diese Datei enthält die Funktion "main". Hier beginnt und endet die Ausführung des Programms.
//

#include "pch.h"
#include "Shape.h"
#include "list"
#include "LShapes.h"
#include "Vector2.h"
#include "cstdlib"
#include "unordered_map"
#include <iostream>

void initWorld();
void UpdateWorldWithShape(Shape &SettedShape, Vector2 &RandomLocation,std::list<Vector2>& ShapeLocations);
bool getElement(int row, int column);
Shape GetRandomShapeElement(std::list<Shape> shapelist);
Vector2 GetRandomMapLocation();

int main()
{
    std::cout << "Hello World!\n"; 
	initWorld();

}

const int xLength = 26;
const int yLength = 29;
bool world[xLength * yLength];

struct hash_fn {
	std::size_t operator()(const Vector2& vec) const {
		std::size_t h1 = std::hash<int>()(vec.x + vec.y);
		return h1 ^ h1;
	}
};
std::unordered_map<Vector2, bool, hash_fn> WorldWithVectors;


void initWorld() {

	//28 x31
	
	for (bool& val : world)val = true;
	for (int i = 0; i < xLength*yLength;++i)
	{
		int xVal = i % xLength;
		int yVal = i / xLength;
		WorldWithVectors[Vector2(xVal, yVal)] = true;
	}

	std::list<Shape> AllShapes = std::list<Shape>();
	
	//Init All Shapes
	LShapes l_shapes = LShapes();
	
	std::list<Shape> LShapes = l_shapes.GetAllShapes();
	AllShapes.insert(AllShapes.end(), LShapes.begin(), LShapes.end());
	//l_shapes->free();
	
	//Set Random Shape
	
	for (int i = 0; i < 50; ++i) {			
		Shape SettedShape = GetRandomShapeElement(AllShapes);			
		Vector2 RandomLocation = GetRandomMapLocation();
		std::list<Vector2> ShapeLocations = std::list<Vector2>();
		UpdateWorldWithShape(SettedShape, RandomLocation, ShapeLocations);
	}
	//std::cout << newLocation.x << " " << newLocation.y << "\n";
	
	for (int i = 0; i< xLength*yLength;++i)
	{
		int xVal = i % xLength;
		int yVal = i / xLength;
		if (xVal == 0)std::printf("\n");
		if (WorldWithVectors[Vector2(xVal, yVal)]) {
			std::printf("x");
		}
		else {
			std::printf("#");
		}
	}
}


void UpdateWorldWithShape(Shape &SettedShape, Vector2 &RandomLocation,std::list<Vector2>& ShapeLocations)
{	

	
	for (Vector2& vec : SettedShape.ShapeLocations) {
		Vector2 newLocation = vec + RandomLocation;	
		if (WorldWithVectors.find(newLocation) != WorldWithVectors.end() && WorldWithVectors.at(newLocation)) {
			ShapeLocations.push_front(newLocation);
			WorldWithVectors[newLocation] = false;
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
	int yVal = Index / yLength;
	return Vector2(xVal, yVal);

}



// Programm ausführen: STRG+F5 oder "Debuggen" > Menü "Ohne Debuggen starten"
// Programm debuggen: F5 oder "Debuggen" > Menü "Debuggen starten"

// Tipps für den Einstieg: 
//   1. Verwenden Sie das Projektmappen-Explorer-Fenster zum Hinzufügen/Verwalten von Dateien.
//   2. Verwenden Sie das Team Explorer-Fenster zum Herstellen einer Verbindung mit der Quellcodeverwaltung.
//   3. Verwenden Sie das Ausgabefenster, um die Buildausgabe und andere Nachrichten anzuzeigen.
//   4. Verwenden Sie das Fenster "Fehlerliste", um Fehler anzuzeigen.
//   5. Wechseln Sie zu "Projekt" > "Neues Element hinzufügen", um neue Codedateien zu erstellen, bzw. zu "Projekt" > "Vorhandenes Element hinzufügen", um dem Projekt vorhandene Codedateien hinzuzufügen.
//   6. Um dieses Projekt später erneut zu öffnen, wechseln Sie zu "Datei" > "Öffnen" > "Projekt", und wählen Sie die SLN-Datei aus.
