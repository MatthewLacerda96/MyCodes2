#include <stdio.h>
#include <stdlib.h>
int bigger,i,j,mult,size;
main(void){
	while(1){
		bigger=0,i=0,j=0,mult=1;								//resets the values at the beginning of the loop
		//the code multiplies all values of the array by 10^x so they become integers. 'mult' is the 10^x
		scanf("%d",&size);
		if (size==0) exit(1);
		float *array=(float*)malloc(size*sizeof(float));		//malloc instead of array[size] so i can use free(array) later
		for(i=0;i<size;i++) scanf("%f",&array[i]);
		for (i=0;i<size;i++){
			if (array[i]*mult-int(array[i]*mult)>0) {			//if array[i]*mult still isn't integer...
				mult*=10;
				i=0;										//i-- bugs sometimes for me so i used i=0
			}
		}
		if (mult>1)											//avoid redundancy
			for(i=0;i<size;i++) array[i]*=mult;
		for(i=0;i<size;i++) if (array[i]>bigger) bigger=array[i];
		int *aux=(int*)calloc(bigger+1,sizeof(int));
		for(i=0;i<size;i++) aux[int(array[i])]++;			//despite array no longer having floats, the compiler demanded int(array[i]
		
		j=0;												//to me it was easier this way than with 'for' loop. just a matter taste
		i=0;
		while(i<size){
			if (aux[j]>0) {
				aux[j]--;
				array[i]=j;
				i++;
			}else j++;
		}
		if (mult>1)											//void redundancy
			for(i=0;i<size;i++) array[i]/=mult;				//values in the now sorted array get back to their values
		for(i=0;i<size;i++) printf("%3.3f ",array[i]);			//3.3 just to make the printf prettier. can be removed
		printf("\n");
		free(array);
		free(aux);
	}
	/*
	Still can be optimized. Mult can be smaller in some cases
	~working on it. stay tuned haha~
	feel free to use this code but a honorable mention would be nice
	*/
}
