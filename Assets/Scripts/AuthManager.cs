using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    

    public static AuthManager authManagerSingleton { private set; get; }

    int initial_memo_count;


    public List<string> memoList = new List<string>();
    public List<string> quizList = new List<string>();
    public int memoListCount;
    public int quizListCount;

    public int temp_question_count;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });


        

        if (authManagerSingleton == null)
        {
            authManagerSingleton = this;
            DontDestroyOnLoad(this);
        }
        else if (authManagerSingleton != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

    }

    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        //Debug.Log("Logged In");
        //Debug.Log(User.Email);
        //Debug.Log(User.DisplayName);

        
        
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    //Signout
    public void SignOutButton()
    {
        //Debug.Log(User.UserId);
        auth.SignOut();
        //UIManager.instance.LoginScreen();
        //ClearRegisterFeilds();
        //ClearLoginFeilds();
        Debug.Log("Logged Out");
        Destroy(authManagerSingleton);
        SceneManager.LoadScene(sceneName: "LoginRegister");
    }


    public void InitialDataDatabase()
    {
        StartCoroutine(UpdateUsernameAuth(User.DisplayName));
        StartCoroutine(UpdateUsernameDatabase(User.DisplayName));
        StartCoroutine(UpdateUserMemoCountInitialDatabase());
        StartCoroutine(UpdateUserQuizCountInitialDatabase());

        //UpdateInitialQuizScoresForTesting();

        
    }


    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;

            
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            ClearLoginFeilds();
            ClearRegisterFeilds();
            

            SceneManager.LoadScene(sceneName: "Menu");

        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        InitialDataDatabase();

                        warningRegisterText.text = "";
                        warningLoginText.text = "";
                        confirmLoginText.text = "";
                        emailLoginField.text = "";
                        passwordLoginField.text = "";
                        ClearLoginFeilds();
                        ClearRegisterFeilds();
                    }
                }
            }
        }
    }


    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
            Debug.Log("Auth Successful");
        }
    }

    private IEnumerator UpdateUserMemoCountInitialDatabase()
    {
        //Set the currently logged in user username in the database
        int memo_count = 0;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("memo_count").SetValueAsync(memo_count);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateUserQuizCountInitialDatabase()
    {
        //Set the currently logged in user username in the database
        int quiz_count = 0;
        //int quiz_count = 10;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quiz_count").SetValueAsync(quiz_count);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            
        }
    }


    private void UpdateInitialQuizScoresForTesting()
    {


        int[] totalQuestions = { 10, 15, 12, 13, 11, 9, 15, 14, 10, 11 };
        int[] correctQuestions = { 6, 10, 9, 9, 8, 6, 12, 8, 5, 10 };
        int[] pigQuestions = { 2, 3, 2, 4, 3, 2, 4, 4, 2, 3 };
        int[] carQuestions = { 3, 4, 4, 4, 2, 3, 3, 3, 2, 2 };
        int[] boatQuestions = { 2, 3, 4, 4, 3, 2, 4, 4, 2, 3 };
        int[] sheepQuestions = { 3, 5, 2, 1, 3, 2, 4, 3, 4, 3 };
        int[] pigCorrectQuestions = { 2, 2, 1, 2, 2, 1, 4, 2, 1, 2 };
        int[] carCorrectQuestions = { 1, 2, 3, 4, 2, 2, 2, 1, 1, 2 };
        int[] boatCorrectQuestions = { 1, 2, 3, 2, 3, 2, 3, 2, 1, 3 };
        int[] sheepCorrectQuestions = { 2, 4, 2, 1, 1, 1, 3, 3, 2, 3 };

        for (int i = 1; i <= 10; i++)
        {


            temp_question_count = totalQuestions[i - 1];
            
            
            //Debug.Log(totalQuestions[i - 1]);
            //Debug.Log(temp_question_count);
            StartCoroutine(UpdateQuizCorrectAnswersDatabase(correctQuestions[i - 1], i));

            StartCoroutine(UpdateQuizPigCountDatabase(pigQuestions[i - 1], i));
            StartCoroutine(UpdateQuizCarCountDatabase(carQuestions[i - 1], i));
            StartCoroutine(UpdateQuizSheepCountDatabase(sheepQuestions[i - 1], i));
            StartCoroutine(UpdateQuizBoatCountDatabase(boatQuestions[i - 1], i));

            StartCoroutine(UpdateQuizPigCorrectDatabase(pigCorrectQuestions[i - 1], i));
            StartCoroutine(UpdateQuizCarCorrectDatabase(carCorrectQuestions[i - 1], i));
            StartCoroutine(UpdateQuizSheepCorrectDatabase(sheepCorrectQuestions[i - 1], i));
            StartCoroutine(UpdateQuizBoatCorrectDatabase(boatCorrectQuestions[i - 1], i));
            StartCoroutine(UpdateQuizTotalQuestionsDatabase(temp_question_count, i));
        }
    }



    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private void UpdateMemoTextDatabase(string memoTextData)
    {
        //Set the currently logged in user username in the database

        initial_memo_count += 1;

        StartCoroutine(UpdateMemoTextMemoCountDatabase());
        StartCoroutine(UpdateMemoTextMemoDatabase(memoTextData));
    }

    private IEnumerator UpdateMemoTextMemoDatabase(string memoTextData2)
    {
        
        string memo_id = "memo_" + initial_memo_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("memos").Child(memo_id).SetValueAsync(memoTextData2);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateMemoTextMemoCountDatabase()
    {
        

        var DBTask = DBreference.Child("users").Child(User.UserId).Child("memo_count").SetValueAsync(initial_memo_count);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }


    private IEnumerator UpdateUserMemoData(string memoTextData)
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            
            Debug.Log(snapshot.Child("memo_count").Value);
            initial_memo_count = Convert.ToInt32(snapshot.Child("memo_count").Value);
            Debug.Log(initial_memo_count);

            UpdateMemoTextDatabase(memoTextData);

        }
    }

    public void SaveMemo(string memoTextData)
    {
        
        StartCoroutine(UpdateUsernameAuth(User.DisplayName));
        StartCoroutine(UpdateUserMemoData(memoTextData));        
    }

    public void SaveQuizData(int totalQuestion, int correctAnswers, int carCount, int carCorrect, int pigCount, int pigCorrect, int boatCount, int boatCorrect, int sheepCount, int sheepCorrect)
    {
        StartCoroutine(UpdateUsernameAuth(User.DisplayName));
        StartCoroutine(GetUserQuizCount(totalQuestion, correctAnswers, carCount, carCorrect, pigCount, pigCorrect, boatCount, boatCorrect, sheepCount, sheepCorrect));
    }

    private IEnumerator GetUserQuizCount(int totalQuestion, int correctAnswers, int carCount, int carCorrect, int pigCount, int pigCorrect, int boatCount, int boatCorrect, int sheepCount, int sheepCorrect)
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            Debug.Log(snapshot.Child("quiz_count").Value);
            int current_quiz_count = Convert.ToInt32(snapshot.Child("quiz_count").Value);
            Debug.Log(initial_memo_count);

            UpdateQuizDataDatabase(totalQuestion, correctAnswers, carCount, carCorrect, pigCount, pigCorrect, boatCount, boatCorrect, sheepCount, sheepCorrect, current_quiz_count);

        }
    }

    private void UpdateQuizDataDatabase(int totalQuestion, int correctAnswers, int carCount, int carCorrect, int pigCount, int pigCorrect, int boatCount, int boatCorrect, int sheepCount, int sheepCorrect, int current_quiz_count)
    {
        current_quiz_count += 1;
        StartCoroutine(UpdateQuizCountDatabase(current_quiz_count));
        StartCoroutine(UpdateQuizTotalQuestionsDatabase(totalQuestion, current_quiz_count));
        StartCoroutine(UpdateQuizCorrectAnswersDatabase(correctAnswers, current_quiz_count));

        StartCoroutine(UpdateQuizPigCountDatabase(pigCount, current_quiz_count));
        StartCoroutine(UpdateQuizCarCountDatabase(carCount, current_quiz_count));
        StartCoroutine(UpdateQuizSheepCountDatabase(sheepCount, current_quiz_count));
        StartCoroutine(UpdateQuizBoatCountDatabase(boatCount, current_quiz_count));
        
        StartCoroutine(UpdateQuizPigCorrectDatabase(pigCorrect, current_quiz_count));
        StartCoroutine(UpdateQuizCarCorrectDatabase(carCorrect, current_quiz_count));
        StartCoroutine(UpdateQuizSheepCorrectDatabase(sheepCorrect, current_quiz_count));
        StartCoroutine(UpdateQuizBoatCorrectDatabase(boatCorrect, current_quiz_count));
        

    }

    private IEnumerator UpdateQuizCountDatabase(int current_quiz_count)
    {


        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quiz_count").SetValueAsync(current_quiz_count);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizTotalQuestionsDatabase(int number, int current_quiz_count)
    {
        //Debug.Log(number);
        //Debug.Log(current_quiz_count);
        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("total_questions").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizCorrectAnswersDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("correct_answers").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizPigCountDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("pig_count").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizCarCountDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("car_count").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizSheepCountDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("sheep_count").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizBoatCountDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("boat_count").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    //Update Correct Responses

    private IEnumerator UpdateQuizPigCorrectDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("pig_correct").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizCarCorrectDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("car_correct").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizSheepCorrectDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("sheep_correct").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateQuizBoatCorrectDatabase(int number, int current_quiz_count)
    {

        string quiz_id = "quiz_" + current_quiz_count;
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("quizes").Child(quiz_id).Child("boat_correct").SetValueAsync(number);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    public void LoadMemoData()
    {
        StartCoroutine(UpdateUsernameAuth(User.DisplayName));
        StartCoroutine(StoreMemoData());
    }

    public void LoadScoresData()
    {
        StartCoroutine(UpdateUsernameAuth(User.DisplayName));
        StartCoroutine(StoreScoresData());
    }

    private IEnumerator StoreScoresData()
    {
        //Get all the users data ordered by kills amount
        
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            /*foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }*/

            //Loop through every users UID
            int x = Convert.ToInt32(snapshot.Child("quiz_count").Value);
            quizListCount = x;
            //memoList = new List<string>();
            string quiz_id;
            for (int i = 1; i <= x; i++)
            {
                quiz_id = "quiz_" + i;
                
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("total_questions").Value.ToString());
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("correct_answers").Value.ToString());
                
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("pig_count").Value.ToString());
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("pig_correct").Value.ToString());

                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("car_count").Value.ToString());
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("car_correct").Value.ToString());

                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("boat_count").Value.ToString());
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("boat_correct").Value.ToString());

                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("sheep_count").Value.ToString());
                quizList.Add(snapshot.Child("quizes").Child(quiz_id).Child("sheep_correct").Value.ToString());
            }
            //Debug.Log(memoListCount);
            //yield return new WaitForSeconds(3);
            SceneManager.LoadScene("ViewScoresPage");
        }
    }


    private IEnumerator StoreMemoData()
    {
        //Get all the users data ordered by kills amount

        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            /*foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }*/

            //Loop through every users UID
            int x = Convert.ToInt32(snapshot.Child("memo_count").Value);
            memoListCount = x;
            //memoList = new List<string>();
            string memo_id;
            for (int i = 1; i <= x; i++)
            {
                memo_id = "memo_" + i;
                memoList.Add(snapshot.Child("memos").Child(memo_id).Value.ToString());

            }
            //Debug.Log(memoListCount);
            //yield return new WaitForSeconds(3);
            SceneManager.LoadScene("MemosAll");
        }
    }


}



