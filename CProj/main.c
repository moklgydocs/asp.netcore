#include<stdio.h>

int main() {
	// 数组指针,实际是一个指针,指向数组的指针
	int arr[] = { 1,3,35,66 };
	int (*ptrArr)[]=&arr;
	printf("*ptrArr[0]=%d\n",(*ptrArr)[0]);

	// 指针数组, 实际是一个数组，存储指针的数组
	int* pArray[] = {1,2,3,5,6};
	printf("pArray[0]=%d\n",pArray[1]);

	return 0;
}