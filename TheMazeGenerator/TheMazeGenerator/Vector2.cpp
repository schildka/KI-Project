#include "pch.h"
#include "Vector2.h"
#include <iostream>


Vector2::Vector2(int x, int y)
{
	this->x = x;
	this->y = y;
}

Vector2 Vector2::operator+(const Vector2 & other)
{
	return Vector2(this->x + other.x, this->y + other.y);
}

Vector2 Vector2::operator-(const Vector2 & other)
{
	return Vector2(this->x - other.x, this->y - other.y);
}

bool Vector2::operator<(const Vector2 & other) {
	return this->x + this->y < other.x + other.y;

}

bool Vector2::operator==(const Vector2 & other) const {
	return (this->x == other.x && this->y == other.y);

}

void Vector2::free()
{
	//delete this;
}

Vector2::~Vector2()
{
}