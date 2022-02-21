![obraz](https://user-images.githubusercontent.com/91827782/154955446-fecacbc4-976e-4cd8-9355-3126235e5f96.png)



1	Design assumptions

  1.	Microcontroller STM32f303k8t6 is a processing unit
  2.	Microcontroller is power supplied from PC via USB
  3.	IMU 10DoF records data: acceleration, angular velocity and magnetic heading
  4.	IMU communicate with microcontroller via I2C bus
  5.	Recorded data are send from microcontroller to PC via UART
  6.	Designed complementary filter in Matlab simulink
  7.	Designed application in .NET framework calculates Euler’s angles from data send from microcontroller and filters data using complementary filter
  8.	Filtered Euler angles values are visualized on the chart
  9.	Sampling rate is 100 Hz


 
2	Components used in project

2.1	Microcontroller STM32 NUCLEO-F303K8T6
•	STM32F303K8T6 in LQFP32 package 
•	ARM®32-bit Cortex®-M4 CPU with FPU 
•	72 MHz max CPU frequency 
•	VDD from 2.0 V to 3.6 V 
•	64 KB Flash 
•	12 KB SRAM 
•	Timers Advanced Control (1) 
•	Timers General Purpose (5) 
•	Basic Timers (2) 
•	SPI/I2S (1) 
•	I2C (1) 
•	USART (2) 
•	CAN (1) 
•	12-bit ADC (2), 9 channels 
•	12-bit DAC (2), 3 channels 
•	GPIO (25) with external interrupt capability 
•	RTC 

                            ![obraz](https://user-images.githubusercontent.com/91827782/154955549-f95df8dc-593b-47d5-a134-72f34a29cfaf.png)

 
                                                                   Figure 1 Microcontroller STM32 F303k8T6

 
2.2	Inertial measurement unit IMU 10DoF (MPU9255+BMP280)

•	Accelerometer, gyroscope, magnetometer
•	VDD: od 3,3 V do 5,5 V
•	Serial communication bus: I2C
•	3 axes: X, Y, Z
•	Dimensions: 31 x 17 mm


                            ![obraz](https://user-images.githubusercontent.com/91827782/154955584-cce840ca-2dab-4ddb-8014-c7d3e2ee9efe.png)

                                                                   Figure 2 IMU 10DoF


                            ![obraz](https://user-images.githubusercontent.com/91827782/154955612-5633f864-010b-4047-b7f0-4c8a6f15472b.png)
 
3	Preliminary draft

Main task of prototype AHRS device is to calculate Euler angles and visualize them in .Net application. IMU collects data about acceleration, magnetic heading and angular velocity in 3 axes. Data are send to MCU via I2C serial communication bus. MCU sends calculated data via UART to .Net application. Application designed in .Net framework visualize Euler angles on the chart.



 
Figure 3 Visualization of prototype


 

