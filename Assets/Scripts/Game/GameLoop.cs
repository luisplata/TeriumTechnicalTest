using SL;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{
    private GameManager _gameManager;
    private TeaTime _start, _lobby, _game, _condition, _end;

    public void Config(GameManager gameManager)
    {
        _gameManager = gameManager;
        _start = this.tt().Pause().Add(() =>
        {
            //init somethings
            _gameManager.Cinematic();
            
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Start begin");
        }).Add(_gameManager.GetTimeFromCinematic()).Add(() =>
        {
            _gameManager.SpawnPlayers();
            _lobby.Play();
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Start end");
        });
        _lobby = this.tt().Pause().Add(() =>
        {
            ServiceLocator.Instance.GetService<IPhotonRPC>().UpdateTable();
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Lobby begin");
            //lobby somethings
            _gameManager.StopInput();
        }).Add(5).Loop(handle =>
        {
            if(_gameManager.GetCountOfPlayers() >= 2)
            {
                handle.Break();
            }
        }).Add(() =>
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Lobby end");
            _game.Play();
        });
        
        _game = this.tt().Pause().Add(() =>
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Game begin");
            //game somethings
            _gameManager.StartInput();
        }).Loop(.5f, handle =>
        {
            handle.Break();
        }).Add(() =>
        {
            _condition.Play();
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Game end");
        });
        
        _condition = this.tt().Pause().Add(() =>
        {
            //condition somethings
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Condition begin");
        }).Loop(handle =>
        {
            if(_gameManager.HowMuchPlayerAlive() <= 1)
            {
                handle.Break();
            }
        }).Add(() =>
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop Condition end");
            _end.Play();
        });
        _end = this.tt().Pause().Add(() =>
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop End begin");
            // show who win
        }).Add(5).Add(() =>
        {
            ServiceLocator.Instance.GetService<IDebug>().Log("GameLoop End end");
            ServiceLocator.Instance.Clear();
            SceneManager.LoadScene(0);
        });
    }

    public void StartGame()
    {
        _start.Play();
    }
}