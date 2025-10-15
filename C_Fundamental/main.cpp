#include "stdio.h"
void swap(int* p, int* q);
int main() {

	int a = 20;
	int b = 30;
	printf("%d\t%d\n", a, b);
	swap(&a, &b);
	printf("%d\t%d",a,b);
	return 0;
}

void swap(int* p, int* q) {
	// 此时*p是变量a的值，*q是变量b的值（*指针 = 访问指针指向的变量）
	*p = *p + *q;  // 第一步：把a+b的和存到a里（此时a=原a+原b，b=原b）
	*q = *p - *q;  // 第二步：用和减b，得到原a，存到b里（此时b=原a，a=原a+原b）
	*p = *p - *q;  // 第三步：用和减新b（原a），得到原b，存到a里（此时a=原b，b=原a）
}