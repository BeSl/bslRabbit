# bslRabbit
библиотека отправки сообщений rabbitMQ

# Установка (PowerShell)
PS C:\l_git\c_vs\myLIbRabbit\myLIbRabbit\bin\Release\netstandard2.0>  C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe myLIbRabbit.dll /codebase

# Пример отправки из 1с

```
	mR = новый COMОбъект("myRabbitClass.BslRabbit");
	mR.SendMessage("текст отправки", "UserName","Password","HostName",
	                port,	Exchange,	Key,	Queue);
	mR = неопределено;
```

