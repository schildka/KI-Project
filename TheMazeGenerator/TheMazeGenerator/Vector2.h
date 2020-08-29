
#pragma once

class Vector2
{

public:
	int x;
	int y;

	Vector2(int x, int y);

	Vector2 operator+(const Vector2& other);
	Vector2 operator-(const Vector2 & other);
	bool operator<(const Vector2 & other);
	bool operator==(const Vector2 & other) const;

	void free();

	~Vector2();
};


