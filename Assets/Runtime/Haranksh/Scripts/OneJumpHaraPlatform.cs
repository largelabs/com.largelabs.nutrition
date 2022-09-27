public class OneJumpHaraPlatform : HaraPlatformAbstract
{
    public override void onCollision()
    {
        //if (gameObject.tag == "Bouncy")
        //{
        //    gameObject.tag = "Untagged";
        //    gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        //}

        //visualObject.color = new Color(1, 1, 1, 0);
        EnableCollider(false);
    }
}
