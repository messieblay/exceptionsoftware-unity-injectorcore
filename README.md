# exceptionsoftware-unity-injectorcore

Inject assets database directly on your MonoBehaviours. Avoiding using Singletons, Gets and Sets or pass your databases through all the code.
Works in both editor and runtime

# How to use
- Inheriting from Monox, Scriptablex, Objectx when you want publish or receibe injection. 
  ![image](https://user-images.githubusercontent.com/79903069/127279649-19a2e2c1-63a5-4c0b-9617-e71b1469056e.png)
  - Static classes don't need any inheriting
  - ![image](https://user-images.githubusercontent.com/79903069/127279505-c22726d9-d2b2-40c9-9467-9956a6b70e11.png)

- Add InjectablexAttribute to Singleton MonoBehaviour, ScriptableObjects or Assets
![image](https://user-images.githubusercontent.com/79903069/127278627-1ff65827-6d35-43ec-9ce8-c98ff31ce03f.png)

- Publish this objects on ExInjector
  - Through code.
   ![image](https://user-images.githubusercontent.com/79903069/127278490-66aaa002-f88e-44fb-8749-59a93f2fb567.png)
  - Inheriting from Monox, Scriptablex, Objectx

- Add InjectxAttribute where you want inject your Assets. Works with fields and properties public, protected and private
![image](https://user-images.githubusercontent.com/79903069/127279560-a84da085-88c6-4954-98f5-eb6d2f615aba.png)
![image](https://user-images.githubusercontent.com/79903069/127279323-b3ab9b50-8892-417a-bdbd-990f3fb00da0.png)


The system will inject your assets when the objects awakes, when a new injection are registred and after unity compilation
