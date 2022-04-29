using UnityEngine;
using System.Collections;



public class PhoneInput : MonoBehaviour
{

    SimControllerHeroHuman _controller;
    HumanGameManager _gameManager;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<SimControllerHeroHuman>();
        _gameManager = GetComponent<HumanGameManager>();

        // I would recommoned chaning the 20.0f to a smaller value based on feedback.
        //var swipeRecognizer = new TKSwipeRecognizer(TKSwipeDirection.All, 20.0f, 50.0f);


        var swipeRecognizer = new TKSwipeRecognizer();
        //Debug.Log("Time to swipe: " + swipeRecognizer.timeToSwipe);
        //swipeRecognizer.timeToSwipe = 0.1f;
        //Debug.Log("Time to swipe: " + swipeRecognizer.timeToSwipe);
        // swipeRecognizer.gestureRecognizedEvent += ( r ) =>
        // {
        // 	_controller.TakePhoneSwipe(r);
        // };
        TouchKit.addGestureRecognizer(swipeRecognizer);

        var tapRecognizer = new TKTapRecognizer();
        //tapRecognizer.boundaryFrame = new TKRect( 0, 0, 50f, 50f );
        tapRecognizer.gestureRecognizedEvent += (r) =>
        {
            //Debug.Log("Boop");
            _controller.TakePhoneTap(r);

        };
        TouchKit.addGestureRecognizer(tapRecognizer);
    }
}
