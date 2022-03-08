/* AHRS by Krzysztof Ragan
 *
 * Reading acceleration, angular velocity and magnetic heading from MPU9255 device
 * */

/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2021 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "i2c.h"
#include "usart.h"
#include "gpio.h"
#include "math.h"
/* Private includes ----------------------------------------------------------*/
#include "sd_hal_mpu9250.h"
/* USER CODE BEGIN Includes */
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
I2C_HandleTypeDef hi2c1;

UART_HandleTypeDef huart2;
/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);

/* USER CODE BEGIN PFP */
/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* Sensor Handler */
/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
   HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_USART2_UART_Init();
  MX_I2C1_Init();
  /* USER CODE BEGIN 2 */


  SD_MPU9250_Result result;
    uint8_t pData[200];
    SD_MPU9250 mpuStruct;
    result = SD_MPU9250_Init(&hi2c1, &mpuStruct, 0, SD_MPU9250_Accelerometer_16G, SD_MPU9250_Gyroscope_2000s);

float Ax,Ay,Az,Gx,Gy,Gz,Mx,My,Mz,psi;
float max_x=0,max_y=0,max_z=0,min_x=0,min_y=0,min_z=0,mean_mx=0,mean_my=0,mean_mz=0;

					/* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
Ax=mpuStruct.Accelerometer_X*mpuStruct.Acce_Mult;
Ay=mpuStruct.Accelerometer_Y*mpuStruct.Acce_Mult;
Az=mpuStruct.Accelerometer_Z*mpuStruct.Acce_Mult;
Gx=mpuStruct.Gyroscope_X*mpuStruct.Gyro_Mult;
Gy=mpuStruct.Gyroscope_Y*mpuStruct.Gyro_Mult;
Gz=mpuStruct.Gyroscope_Z*mpuStruct.Gyro_Mult;
Mx=mpuStruct.Mag_X/0.6;
My=mpuStruct.Mag_Y/0.6;
Mz=mpuStruct.Mag_Z/0.6;
// Calibrating magnetometer
if (Mx>max_x)
{
max_x=Mx;
}
if (My>max_y)
{
max_y=My;
}
if (Mz>max_z)
{
max_z=Mz;
}
if (Mx<min_x)
{
min_x=Mx;
}
if (My<min_y)
{
min_y=My;
}
if (Mz<min_z)
{
min_z=Mz;
}
mean_mx=(max_x+min_x)/2;
mean_my=(max_y+min_y)/2;
mean_mz=(max_z+min_z)/2;
Mx=Mx-mean_mx;
My=My-mean_my;
Mz=Mz-mean_mz;
//end of calibration
result = SD_MPU9250_ReadAll(&hi2c1, &mpuStruct);
printf("%f;%f;%f;%f;%f;%f;%f;%f;%f\n",Ax,Ay,Az,Gx,Gy,Gz,Mx,My,Mz);
//printf("%f;%f;%f;%f;%f;%f;%f;%f;%f\r\n",Ax,Ay,Az,Gx,Gy,Gz,Mx,My,Mz);

//printf("%f",Ax);
					HAL_Delay(100);
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};
  RCC_PeriphCLKInitTypeDef PeriphClkInit = {0};

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_0) != HAL_OK)
  {
    Error_Handler();
  }
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_I2C1;
  PeriphClkInit.I2c1ClockSelection = RCC_I2C1CLKSOURCE_HSI;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */

  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
