using UnityEngine;
using UnityEngine.UIElements;
public class ColonyRegression : MonoBehaviour
{
    public string Result = "not run";
#if UNITY_EDITOR
    System.Text.StringBuilder evidence = new System.Text.StringBuilder();
    int failures;
    void Check(bool ok, string name) { evidence.AppendLine((ok ? "PASS " : "FAIL ") + name); if (!ok) failures++; }
    void Click(string name)
    {
        var doc = Object.FindFirstObjectByType<UIDocument>();
        var b = UQueryExtensions.Q<Button>(doc.rootVisualElement, name);
        using (var e = ClickEvent.GetPooled()) { e.target = b; b.SendEvent(e); }
    }
    public void Run() { StartCoroutine(Tests()); }
    System.Collections.IEnumerator Tests()
    {
        Result = "running";
        var d = ColonyGameDirector.Instance;
        Click("play-btn");
        yield return null;
        Check(d.IsPlaying && d.PlayerHealth.current == 100 && d.food == 0 && d.Nest != null && d.Nest.Health.current == 360, "menu Play starts fresh raid with nest");
        var p = d.Player; var start = p.transform.position; var rotation = p.transform.rotation;
        p.enabled = false;
        float t = 0;
        while (t < .7f) { p.Drive(Vector2.up, new Vector2(.15f,0), false,false);t+=Time.deltaTime;yield return null; }
        Check(Vector3.Distance(start,p.transform.position)>1f && Quaternion.Angle(rotation,p.transform.rotation)>1f,"movement and mouse-look shared input path");
        float baseY=p.transform.position.y;
        p.Drive(Vector2.zero,Vector2.zero,false,true);
        t=0;float high=baseY;
        while(t<1.3f){p.Drive(Vector2.zero,Vector2.zero,false,false);high=Mathf.Max(high,p.transform.position.y);t+=Time.deltaTime;yield return null;}
        Check(high>baseY+.3f && p.IsGrounded,"jump rises and lands on floor");
        var crumbs=Object.FindObjectsByType<ColonyFoodPickup>(FindObjectsSortMode.None);
        foreach(var crumb in crumbs) crumb.ResetPickup();
        d.food=0;
        for(int i=0;i<3;i++)
        {
            var pos=crumbs[i].transform.position;pos.y=.08f;p.ResetMotor(pos,Quaternion.identity);
            p.Drive(Vector2.zero,Vector2.zero,false,false);yield return new WaitForSeconds(.12f);
        }
        Check(d.food==3,"physical food overlaps collect three crumbs");
        Check(d.TrySummon() && d.food==0 && d.SoldierCount==1,"summon spends 3 food and creates ally");
        var ally=Object.FindFirstObjectByType<ColonySoldierAI>();
        float hp=ally.GetComponent<ColonyHealth>().current;
        Check(!ally.GetComponent<ColonyHealth>().ApplyDamage(10,p.gameObject) && ally.GetComponent<ColonyHealth>().current==hp,"allied damage is blocked");
        var anim=ally.GetComponentInChildren<Animator>();
        ally.enabled=false;
        ally.GetComponent<ColonyAntMotion>().SetMoving(true,false);
        yield return new WaitForSeconds(.15f);
        float a=anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        yield return new WaitForSeconds(.2f);
        float b=anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        Check(anim.runtimeAnimatorController!=null && anim.avatar!=null && b!=a,"skeletal animation clock advances");
        ally.enabled=true;
        d.food=2;
        Click("spawn-collector-btn");
        yield return null;
        Check(d.CollectorCount==1 && d.food==0,"HUD collector button spends 2 food");
        var collector=Object.FindFirstObjectByType<ColonyCollectorAI>();
        Check(collector!=null && collector.GetComponent<ColonyClimber>()!=null,"collector has wall climber");
        d.PauseGame();Check(d.flow==ColonyGameDirector.Flow.Paused && Time.timeScale==0,"pause freezes gameplay");
        Click("resume-btn");Check(d.IsPlaying && Time.timeScale==1,"resume button restores gameplay");
        yield return new WaitForSeconds(4.2f);
        Check(d.AliveRaiders>0,"anthill spawns raiders");
        var raider=Object.FindFirstObjectByType<ColonyRaiderAI>();
        p.ResetMotor(new Vector3(0,.12f,-12),Quaternion.identity);
        var cc=raider.GetComponent<CharacterController>();cc.enabled=false;raider.transform.position=new Vector3(0,.12f,-7);cc.enabled=true;
        d.PlayerHealth.current=100;d.PlayerHealth.regenPerSecond=0;
        yield return new WaitForSeconds(2.5f);
        Check(d.PlayerHealth.current<100,"enemy AI fires damaging projectiles");
        d.PlayerHealth.invulnerable=true;
        float nestBefore=d.Nest.Health.current;
        var far=d.Nest.transform.position+d.Nest.transform.forward*1.6f+Vector3.up*.2f;
        p.ResetMotor(new Vector3(8,.12f,8),Quaternion.identity);
        cc.enabled=false;raider.transform.position=far;cc.enabled=true;
        yield return new WaitForSeconds(2.2f);
        Check(d.Nest.Health.current<nestBefore || d.FindRaiderTarget(raider.transform.position)==d.Nest.transform,"raiders attack the player nest");
        Vector3 climbStart=new Vector3(0f,.12f,-17.15f);
        p.ResetMotor(climbStart,Quaternion.LookRotation(Vector3.back));
        var climber=p.GetComponent<ColonyClimber>();
        t=0;float climbY=p.transform.position.y;bool climbed=false;
        while(t<1.6f){p.Drive(Vector2.up,Vector2.zero,false,false);if(climber!=null&&climber.Climbing)climbed=true;climbY=Mathf.Max(climbY,p.transform.position.y);t+=Time.deltaTime;yield return null;}
        Check(climbed && climbY>climbStart.y+.35f,"player climbs kitchen walls");
        p.ResetMotor(new Vector3(13.5f,.1f,-2),Quaternion.identity);
        var gun=p.GetComponent<ColonyAcidGun>();
        t=0;int fired=0;
        while(d.IsPlaying && t<12){if(gun.TryFire(d.Anthill.GetComponent<ColonyHealth>().AimPoint-gun.Origin))fired++;t+=Time.deltaTime;yield return null;}
        Check(d.flow==ColonyGameDirector.Flow.Won,"player projectiles destroy hill and win (shots="+fired+")");
        int count=d.AliveRaiders;yield return new WaitForSeconds(.5f);Check(d.AliveRaiders==count,"destroyed base stops spawning");
        Click("result-menu-btn");Check(d.flow==ColonyGameDirector.Flow.Menu,"result menu has no recursive event loop");
        Click("play-btn");yield return null;
        Check(d.food==0 && d.SoldierCount==0 && d.CollectorCount==0 && d.AliveRaiders==0 && d.Anthill.GetComponent<ColonyHealth>().current==420 && d.Nest.Health.current==360,"replay clears units, food, nest and hill damage");
        Check(Vector3.Distance(p.transform.position,start)<.35f,"replay restores player spawn");
        d.PlayerHealth.ApplyDamage(999,null);yield return null;
        Check(d.IsPlaying && d.PlayerHealth.current==100 && Vector3.Distance(p.transform.position,d.Nest.RespawnPosition)<1.2f,"player death respawns at nest");
        d.Nest.Health.ApplyDamage(999,null);yield return null;
        Check(d.flow==ColonyGameDirector.Flow.Lost,"destroyed nest triggers defeat");
        Click("restart-btn");yield return null;
        Check(d.IsPlaying && d.PlayerHealth.current==100 && d.Nest.Health.current==360,"defeat restart restores health and nest");
        d.ReturnToMenu();p.enabled=true;
        Result=(failures==0?"PASSED":"FAILED")+"\n"+evidence;
    }
#endif
}
