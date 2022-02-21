![obraz](https://user-images.githubusercontent.com/91827782/154970055-b5efe2ac-b94e-4665-9e20-9a80f040c86a.png)


#1	Design assumptions

1.	Microcontroller STM32f303k8t6 is a processing unit
2.	Microcontroller is power supplied from PC via USB
3.	IMU 10DoF records data: acceleration, angular velocity and magnetic heading
4.	IMU communicate with microcontroller via I2C bus
5.	Recorded data are send from microcontroller to PC via UART
6.	Designed complementary filter in Matlab simulink
7.	Designed application in .NET framework calculates Euler’s angles from data send from microcontroller and filters data using complementary filter
8.	Filtered Euler angles values are visualized on the chart
9.	Sampling rate is 100 Hz

#2	Components used in project

##2.1	Microcontroller STM32 NUCLEO-F303K8T6

-	STM32F303K8T6 in LQFP32 package 
-	ARM®32-bit Cortex®-M4 CPU with FPU 
-	72 MHz max CPU frequency 
-	VDD from 2.0 V to 3.6 V 
-	64 KB Flash 
-	12 KB SRAM 
-	Timers Advanced Control (1) 
-	Timers General Purpose (5) 
-	Basic Timers (2) 
-	SPI/I2S (1) 
-	I2C (1) 
-	USART (2) 
-	CAN (1) 
-	12-bit ADC (2), 9 channels 
-	12-bit DAC (2), 3 channels 
-	GPIO (25) with external interrupt capability 
-	RTC 

![obraz](https://user-images.githubusercontent.com/91827782/154970187-4d8d2fcc-7e6d-4798-a5ae-c0bde61e8bf1.png)

podpis

##2.2	Inertial measurement unit IMU 10DoF (MPU9255+BMP280)

-	Accelerometer, gyroscope, magnetometer
-	VDD: od 3,3 V do 5,5 V
-	Serial communication bus: I2C
-	3 axes: X, Y, Z
-	Dimensions: 31 x 17 mm

![obraz](https://user-images.githubusercontent.com/91827782/154970284-3146150a-2c28-4735-99d1-7e5ca36c7ad4.png)

podpis

#3	Preliminary draft

Main task of prototype AHRS device is to calculate Euler angles and visualize them in .Net application. IMU collects data about acceleration, magnetic heading and angular velocity in 3 axes. Data are send to MCU via I2C serial communication bus. MCU sends calculated data via UART to .Net application. Application designed in .Net framework visualize Euler angles on the chart.

![obraz](https://user-images.githubusercontent.com/91827782/154970343-1bf130db-adfc-4af2-8ead-ccda8c1183cf.png)
podpis

