# LiteBind

LiteBind — это лёгкий, минималистичный и расширяемый DI-контейнер для Unity.  
Вдохновлён Zenject, но проще, понятнее и с полным контролем.

---

## 🚀 Возможности

- ✅ Простая регистрация Singleton и Transient
- ✅ Инъекции через атрибут `[LiteInject]`
- ✅ Автоматическая поддержка `IInitializable`, `IUpdatable`, `IDisposableService`
- ✅ Поддержка фабрик `IFactory<TParam, TResult>`
- ✅ Контейнеры сцены и проекта с наследованием
- ✅ Кастомные иконки
- ✅ Без зависимостей — чистый C#

---

## 📦 Установка

Добавьте в `manifest.json` вашего проекта:
"com.askar.litebind": "https://github.com/Askar2702/LiteBind.git"


🧠 Быстрый старт
1. Создайте LiteProjectContext в сцене
```csharp
public class ProjectRoot : LiteProjectContext
{
    protected override void InstallBindings(LiteBindContainer container)
    {
        container.BindSingleton<IAnalyticsService>(new AnalyticsService());
    }
}
```
2. Создайте LiteSceneContext в сцене
```csharp
public class GameSceneContext : LiteSceneContext
{
    protected override void InstallBindings(LiteBindContainer container)
    {
        container.BindSingletonInterfaceAndSelf<IPlayerService, PlayerService>(new PlayerService());
    }
}
```
3. Используйте [LiteInject]
```csharp
public class PlayerHUD : MonoBehaviour
{
    [LiteInject] private IPlayerService _playerService;

    private void Start()
    {
        _playerService.DoSomething();
    }
}
```
🧬 Жизненный цикл
Если ваш сервис реализует:
```csharp
public interface IInitializable { void Initialize(); }
public interface IUpdatable { void Tick(); }
public interface IDisposableService { void Dispose(); }
Он будет вызываться автоматически системой LiteLifecycleRunner.
```
🏭 Фабрики

```csharp
container.BindFactory<string, Enemy>(name => new Enemy(name));

var factory = container.Resolve<IFactory<string, Enemy>>();
var enemy = factory.Create("Zombie");
```
📛 Атрибуты
[LiteInject] — внедряет зависимости в поля MonoBehaviour (автоматически)

LiteBindContainer.InjectInto(myMono) — для ручного внедрения

🧰 Автор
Askar
Telegram: [@https://t.me/Askar_27]
GitHub: Askar2702
