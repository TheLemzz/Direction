<br />
<div align="center">
  <a href="https://github.com/othneildrew/Best-README-Template">
    <img src="https://i.imgur.com/BJWVVvC.jpeg" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Direction: Среда симуляции</h3>

  <p align="center">
    Удобная среда для тестирования автопилотов с удобной интеграцией Python!

<details>
  <summary>Навигация:</summary>
  <ol>
    <li>
      <a href="#о-проекте">О проекте</a>
      <ul>
      </ul>
    </li>
    <li>
      <a href="#начало-работы">Начало работы</a>
      <ul>
        <li><a href="#установка">Требуемые компоненты</a></li>
      </ul>
    </li>
    <li><a href="#использование">Использование</a></li>
    <li><a href="#roadmap">Планы на будущее</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## О проекте

<img  src="https://github.com/Anmol-Baranwal/Cool-GIFs-For-GitHub/assets/74038190/398b19b1-9aae-4c1f-8bc0-d172a2c08d68"  width="45">Разработка различных ассистентов или автопилотов для автомобилей - дорогостоящее удовольствие,
тем более, когда речь заходит о тестировании данных продуктов реальных условиях.
Данный проект помогает протестировать продукт в симуляции города,
 приближенной к реальности. Реализовано:
* Поведение пешеходов
* Поведение сторонних машин
* Управление собственной машиной
* Погодные условия: дождь, туман
* Процедурная генерация ям на дорожном покрытии

Данный проект решает дороговизну тестирования и упрощает процесс разработки подобных продуктов. Данная симуляция изначально создавалась под тестирование продукта ***'AiAssistent - ассистент безопасной скорости'***  для [больших вызовов.](https://konkurs.sochisirius.ru/)

<!-- GETTING STARTED -->
## Начало работы

<img  src="https://user-images.githubusercontent.com/74038190/212284087-bbe7e430-757e-4901-90bf-4cd2ce3e1852.gif"  width="50"> Симуляция реализуется на небезизвестном движке [Unity](https://unity.com/) *(2022.3.11f1)* на языке C#:
* Unity гораздо легче в освоении
* В качестве языка для разработки используется не столь сложный C#.
* Доступный и понятный интерфейс

 В дальнейшем планируется открыть ***Direction*** на UnrealEngine для улучшения реалистичности.

![](https://p1-tt-ipv6.byteimg.com/origin/pgc-image/2781319826964cd5b957f38cfedb4370)
### Установка

**- Unity:**
Достаточно установить [Unity Hub](https://unity.com/download) и соответствующую версию движка, используемая для ***Direction*** [(2022.3.11f1)](unityhub://2022.3.11f1/d00248457e15)

**- Python:**
***Direction*** подерживает любую версию [Python](https://www.python.org/downloads/). Путь до интерпетатора Вы указываете самостоятельно. Если ваш продукт не использует Python в разработке, то вы можете пропустить этот пункт.

**- С#:**
Для работы с C#-скриптами рекомендуется [Visual Studio](https://visualstudio.microsoft.com/ru/vs/older-downloads/), который можно установить прямо из Unity:
![f](https://i.imgur.com/eshLhwK.png)

Либо можете использовать любой другой удобный Вам редактор.

**- Direction:**
Установите сам проект Direction посредством git clone:

    git clone https://gitlab.com/TheLemzzy/direction.git
   Затем, используя ранее установленный UnityHub, добавьте папку проекта в список проектов:
   ![Direction installation](https://i.imgur.com/9y4cqfA.png)

## Использование
![Unity](https://i.imgur.com/bvfrqqY.png)
<img src="https://media.giphy.com/media/iY8CRBdQXODJSCERIr/giphy.gif"  width="30px"> Ниже будут расписаны разделы работы с Direction.
### Интеграция скриптов Python - PyModule.cs:

![Python](https://i.imgur.com/tII4H63.png)

<img src="https://media4.giphy.com/media/UtEd87cLAH789bR5sk/giphy.gif?cid=6c09b9524ctyfn5hy2vdigdiut0mr5fn88ek5v0298wjpeqk&ep=v1_internal_gif_by_id&rid=giphy.gif&ct=s" width="50">**PyModule** - основной скрипт, закрепленный на объекте **ProjectManager**, через который происходит работа со скриптами Python.
1. *InterpretatorPath*: путь до интерпретатора Python.
2. *Script Path*: путь до скрипта Python.
3. *Show Output*: Если включено, после завершения работы выведет все логи\ошибки, выброшенные скриптом Python.
4. *Hide Console*: Запуск скриптов осуществляется через командную строку. Если включено, то она не будет отображаться.
5. *Eternal*: Включите, если ваш скрипт Python должен работать весь цикл симуляции(РЕАЛИЗУЕТСЯ НА СТОРОНЕ PYTHON). После завершения работы симуляции, модуль создаст файл остановки в рабочей директории скрипта. 
6. *Work Path*: Рабочая директория скрипта Python. Там будет создан файл остановки. *Можно оставить пустым, если Eternal = false.*

7. *Args*: Аргументы для запуска скрипта Python. Можно оставить пустым.

Алгоритм работы прост: достаточно добавить в список скриптов  Python новый элемент, указать необходимые данные и всё - PyModule запустит их в указанном порядке и сам выключит, если нужно.

Пример обработки файла остановки на Python:
```python
if os.path.exists(datas_folder):  
while True:  
	for filename in os.listdir(datas_folder):  
		filepath = os.path.join(datas_folder, filename)  
		if filename.endswith('.jpg'):  
			process_screenshot(filepath)  
		elif 'stop' in filename:  
			with open(output_file, 'w') as f: f.write('0')  
			os.remove(os.path.join(datas_folder, filename))  
			break
```
PyModule реализует **Singleton.**

```csharp
public static PyModule Instance => _instance;
```

 К PyModule можно обратиться через PyModule.Instance. **Делать это возможно только после запуска компонентов симуляции!**  Самый простой способ - подписаться на событие **EntryPoint.OnApplicationStarted**:

```csharp
private void OnEnable()
{
EntryPoint.OnApplicationStarted += OnApplicationStarted;
}

private void OnDisable()
{
EntryPoint.OnApplicationStarted -= OnApplicationStarted;
}

private void OnApplicationStarted()
{
PyModule pyModule = PyModule.Instance;
/// other code here...
}
```
 Возможно, вы захотите внести некую логику в PyModule. Например, в реализации **AIAssistent** PyModule использовался для удобного хранения рабочих путей скрпитов Python:
```csharp
public string GetDetectorDataPath()
{
	return @"E:\UnityProjects\siriusinternal\AI\datas_people\";
}
    
public string GetRoadDataPath()
{
	return @"E:\UnityProjects\siriusinternal\AI\datas\";
}
```
И использование:
```csharp
private IEnumerator Photo()
{
	WaitForSeconds wait = new(0.4f);
    
	while (true)
	{
		yield return wait;
		_cameraRecorder.MakePhoto(false, PyModule.Instance.GetDetectorDataPath());
	}
}
```
### CarCameraRecorder.cs:
![Camera](https://i.imgur.com/Mq8bkbZ.png)
CarCameraRecorder - MonoBehaviour, который прикреплен к рабочей камере автомобиля. Можно прикрепить на любую другую камеру.

У этого MonoBehaviour есть полезный метод - **MakePhoto()**, который  делает снимок с камеры и сохраняет его в указанную директорию.

    public string MakePhoto(bool forRoad = false, string directory = "")
   forRoad - если true, то снимок будет сделан с другой камеры, направленный вниз на дорожное покрытие. 
   directory - путь сохранения снимка.
   
   *Пример использования:*
```csharp
private IEnumerator SendRoadData()
{
	WaitForSeconds wait = new(2.2f);
    
	while (true)
	{
		yield return wait;
		TrySendRoadData();
	}
}
    
private void TrySendRoadData()
{
	if (_sendData && _car.GetCarSpeed() >= 3) _cameraRecorder.MakePhoto(true, _module.GetRoadDataPath());
}
```
   **AIAssistant**: Данный фрагмент кода каждые 2.2с пытается сделать снимок с камеры дорожного покрытия и сохранить его в рабочей директории скрипта детектирования трещин дороги.

### Car.cs:
![Car.cs](https://i.imgur.com/Caswbsq.png)

**Car.cs** - базовый класс автомобиля для управления им. Можно использовать свои InputHandler для управления автомобилем. В проекте реализовано управление игроком (*PlayerInputHandler.cs*) и управление ботом (*AIController2.cs*)
Car.cs имеет полезный метод - **GetCarSpeed()**, который возвращает текущую скорость автомобиля(int).

```csharp
carSpeedText.text = _car.GetCarSpeed().ToString();
```

### WeatherManager.cs:
![Weather](https://i.imgur.com/MaJ1bFH.png)

**WeatherManager.cs** - класс, ответственный за смену погоды. Он вызывает эвент **OnWeatherChanged**, который первым параметром передает bool-значение, обозначающее, стартовала или закончилась погода, а вторым - enum **WeatherType**, характеризующий тип погоды.

Пример использования:
```csharp
private void OnEnable()
{
	WeatherManager.OnWeatherChanged += OnWeatherChanged;
}
    
private void OnDisable()
{
	WeatherManager.OnWeatherChanged -= OnWeatherChanged;
}
    
private void OnWeatherChanged(bool started, WeatherType weatherType)
{
	if (started)
	{
		switch (weatherType)
		{
			case WeatherType.Rain:
			_carController.AllowedSpeed -= _coefficentRain;
			_intelligentSystem.SendWarningAllert(RAIN_WARNING);
			break;
			case WeatherType.Fog:
			    _carController.AllowedSpeed -= _coefficentFog;
			    _intelligentSystem.SendWarningAllert(FOG_WARNING);
			    break;
			default:
			    return;
		}
	}
	else
	switch (weatherType)
	{
		case WeatherType.Rain:
	                _carController.AllowedSpeed += _coefficentRain;
	                _intelligentSystem.DeleteWarningWithDescription(RAIN_WARNING);
	                break;
            	case WeatherType.Fog:
	                _carController.AllowedSpeed += _coefficentFog;
	                _intelligentSystem.DeleteWarningWithDescription(FOG_WARNING);
	                break;
            	default:
                	return;
	}
}
```
 
 ### WarningAlertHolder.cs:
![Container](https://i.imgur.com/zgxCyFP.png)

**WarningAlertHolder.cs** - MonoBehaviour, реализующий удобный вывод предупреждений на Canvas пользователя.

Методы:
```csharp
public WarningAlert AddWarningAlert(string description)
```
Добавляет новое предупреждение с указанным описанием на экран. Возвращает созданный *WarningAlert*. Может быть создано только одно предупреждение с указанным описанием.

```csharp
public void TryRemoveWarningAlert(string data)
```
Удаляет предупреждение, у которого есть данное описание, если оно есть в списке.

```csharp
public bool IsHasWarningWithDescription(string description)
```
Возвращает true, если в списке есть предупреждение с указанным описанием.

Примеры использования:
```csharp
private const string SPEED_WARNING = "Скорость превышена!";

private void FixedUpdate()
{
    if (!init) return;

    if (_carController.IgonreRule) CheckSpeed();

    if (_car.GetCarSpeed() < _carController.AllowedSpeed)
    {
        DeleteWarningWithDescription();
    }
}

private void CheckSpeed()
{
    if (_car.GetCarSpeed() > _carController.AllowedSpeed)
    {
        SendWarningAlert(SPEED_WARNING);
    }
}

public void SendWarningAlert(string description)
{
    _warningAllertHolder.AddWarningAlert(description);
}

public void DeleteWarningWithDescription(string data = SPEED_WARNING)
{
    _warningAllertHolder.TryRemoveWarningAlert(data);
}
```
Удобно записать описание предупреждения в const, а затем использовать ее для удаления или создания предупреждения.


### Построение путей автомобилей-ботов:
![пути](https://i.imgur.com/dON06IM.png)

В проекте уже есть заготовленный автомобиль-бот: **CarAI**. Будем использовать его для построения маршрута...

Маршрут формируется следующим образом:

 1. Создается пустой родительский GameObject на сцене
 2. В нем создаются пустые GameObject`ы - они выступают в роли точек.
 3. После создания всех точек, на родительский GameObject добавляется скрипт **Circuit.cs**, настраивается цвет. можно в дальнейшем добавить свои тчоки в лист Waypoints.
 
 Путь должен быть цикличен, т.е. из последней точки автомобиль должен прийти к первой точке. 
 **Color** - это параметр цвета, которым будет отображаться основной путь в Editor Mode. **Зеленая линия - линия, соединяющая конец и начало пути.** Примерно по линиям будет двигаться автомобиль. Учитывайте угол поворота и скорость поворта автомобиля, чтобы у машину не заносило.
 *CarAI будет автоматически останавливаться, если спереди будет обнаружен автомобиль.*

В *CarAI -> AIController2* есть поле *Circuit* - в него добавляем созданный путь. Готово!
![AIController](https://i.imgur.com/uPxZ3GX.png)
У нескольких машин может быть одинаковый маршрут.

### Построение маршрута пешеходов:
![Пешеходы](https://i.imgur.com/8ibu1H7.png)

**Human.cs** - скрипт, реализующий логику пешеходов. 
* *PosRight* - позиции первой стороны пешеходного перехода
* *PosLeft* - позиции второй стороны пешеходного перехода 
* *State* - На какой позиции сейчас находится пешеход. Если Left, то он пойдет в позиции типа Right и  наоборот.

Принцип схож с построением маршрута автомобилей - создайте пустой дочерний объект, в нем создайте позиции для каждой стороны и добавьте их в лист позиций пешехода. 
Есть два типа готовых пешеходов - **Man** и **Woman**, можете использовать их.

### ProductInfo:
![product](https://i.imgur.com/CrkG8my.png)

**ProductInfo** - [ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html), позволяющий менять информацию о тестируемом продукте в *Scenes -> MenuScene -> Canvas -> Product*.
![AIAssistant](https://i.imgur.com/hcZV3qy.png) 

Можно создавать свою информацию о продуктах:
![products](https://i.imgur.com/HNDfsn7.png)

Информация о продукте выводится в меню при запуске проекта. 

## Roadmap

- [x] Реализовать AIAssistant
- [x] Упростить интеграцию Python
- [ ] Сделать перевод на несколько языков
- [ ] Доработать графику
- [ ] Снегопад
- [ ] Разработать версию Direction на UnrealEngine


## Сторонняя разработка
**Direction** на данный момент находится в открытом доступе и открыт к внешним изменениям и доработкам, за исключением коммерческого использования без согласия правообладателя.


# Заключение
*Вы можете детально рассмотреть реализацию AIAssistant в данной симуляции, если у остались моменты, которые вам непонятны. Открывайте issue, если нашли проблемы!*

<img src="https://user-images.githubusercontent.com/74038190/225813708-98b745f2-7d22-48cf-9150-083f1b00d6c9.gif" width="1000">
