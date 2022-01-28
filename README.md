**Test task from preinterview 3**  

[![.NET Build](https://github.com/vnmtwo/SerializeDeserializeTestTask/actions/workflows/dotnet_build.yml/badge.svg)](https://github.com/vnmtwo/SerializeDeserializeTestTask/actions/workflows/dotnet_build.yml)
[![.NET Test](https://github.com/vnmtwo/SerializeDeserializeTestTask/actions/workflows/dotnet_test.yml/badge.svg)](https://github.com/vnmtwo/SerializeDeserializeTestTask/actions/workflows/dotnet_test.yml)  

- took to create fisrt version: 1:40  
- took to create test and debug: 0:40
- took to create second version, test, debug: 4:00 (lost in references)

**Задание**  

Реализуйте функции сериализации и десериализации двусвязного списка, заданного следующим образом:  

    class ListNode
    {
      public ListNode Previous;
      public ListNode Next;
      public ListNode Random; // произвольный элемент внутри списка
      public string Data;
    }
    
    class ListRandom
    {
      public ListNode Head;
      public ListNode Tail;
      public int Count;

      public void Serialize(Stream s)
      {
      }

      public void Deserialize(Stream s)
      {
      }
    }
  
Примечание: сериализация подразумевает сохранение и восстановление полной структуры списка,
включая взаимное соотношение его элементов между собой.
Напишите программу, демонстрирующую работу реализованных функций сериализации и
десериализации на небольшом наборе тестовых данных (списке из нескольких элементов). Тест
нужно выполнить без использования библиотек/стандартных средств сериализации. Сигнатуры
методов serialize/deserialize менять нельзя.
