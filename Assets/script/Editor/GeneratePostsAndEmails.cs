using UnityEngine;
using UnityEditor;
using System.IO;

public class GeneratePostsAndEmails
{
    [MenuItem("Moderator/Generate All Posts and Emails")]
    public static void Generate()
    {
        string postFolder  = "Assets/Data/Posts";
        string emailFolder = "Assets/Data/Emails";

        if (!Directory.Exists(postFolder))  Directory.CreateDirectory(postFolder);
        if (!Directory.Exists(emailFolder)) Directory.CreateDirectory(emailFolder);

        GeneratePosts(postFolder);
        GenerateEmails(emailFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Done! 40 posts and 5 emails created in Assets/Data/");
    }

    static void GeneratePosts(string folder)
    {
        // --- HARMFUL posts (isHarmful = true) ---
        var harmful = new (string content, string author, string date, int day)[]
        {
            // Day 1 — spam and profanity
            ("BUY NOW!!! Lose 30kg in 2 weeks!!! 100% GUARANTEED or MONEY BACK!!! Click here: sp4m-pills.ru", "@promo_bot_44", "03.12.2034", 1),
            ("This product is absolute trash, the seller is a f***ing scammer, I hope he rots in hell!!!", "@angry_user_92", "03.12.2034", 1),
            ("HOT SINGLES IN YOUR AREA RIGHT NOW CLICK HERE FREE NO CREDIT CARD adult-meet.biz", "@meet_bot_01", "03.12.2034", 1),
            ("You are all IDIOTS if you don't see that this company is stealing from you. MORONS.", "@rage_post_77", "03.12.2034", 1),
            ("Earn $5000 per day working from home!!! No experience needed!!! DM me NOW!!!", "@easy_money_x", "03.12.2034", 1),
            ("This b*tch at customer service was completely useless. Absolute garbage service.", "@frustrated_111", "03.12.2034", 1),
            ("FREE iPHONE 15 GIVEAWAY!! Just share this post and follow @scam_account to win!!!", "@giveaway_bot", "03.12.2034", 1),
            ("I will find you and you will regret what you did to me. Watch your back.", "@anon_threat_x", "03.12.2034", 1),

            // Day 2 — virus / lights mentions (new directive)
            ("Saw strange lights over the city last night. Three of them, completely silent, hovering for 20 minutes. Anyone else?", "@nightwatcher_02", "04.12.2034", 2),
            ("My cousin works at the hospital. Says 12 new virus cases today, all same symptoms. They're not telling us anything.", "@insider_tip_99", "04.12.2034", 2),
            ("The lights are back. I have VIDEO this time. They were right above the power station. DM me for footage.", "@lights_recorder", "04.12.2034", 2),
            ("Unknown virus spreading in the eastern district confirmed by 3 independent sources. Media is silent. Why???", "@truth_seeker_7", "04.12.2034", 2),
            ("Government is covering up the virus outbreak. My doctor friend confirmed 40+ cases unreported.", "@expose_them_now", "04.12.2034", 2),

            // Day 3 — combined, more intense
            ("THE CITY IS BEING QUARANTINED. I just drove past 6 roadblocks. This is NOT a drill. SHARE THIS.", "@breaking_alert", "05.12.2034", 3),
            ("Virus mutated. New strain confirmed. Hospital sources say they ran out of beds 2 days ago.", "@med_leaks_real", "05.12.2034", 3),
            ("PROOF: lights are military drones scanning for infected. They come every night at 2am.", "@conspiracy_node", "05.12.2034", 3),
            ("They are locking down Block 7 through 12 tonight. If you live there GET OUT NOW.", "@urgent_warning", "05.12.2034", 3),
            ("My neighbor collapsed this morning. Ambulance took 3 hours. They wore full hazmat suits.", "@scared_local_4", "05.12.2034", 3),

            // Day 4 — reality glitch posts
            ("Moderator @user_4471 has been deleting posts about the outbreak for 3 days. He knows. #ModeratorExposed", "@watch_the_mods", "06.12.2034", 4),
            ("The person approving fake news on this platform is named in leaked documents. They are paid to lie.", "@leak_source_1", "06.12.2034", 4),
        };

        // --- SAFE posts (isHarmful = false) ---
        var safe = new (string content, string author, string date, int day)[]
        {
            // Day 1
            ("Just made homemade sourdough bread for the first time. Took 3 days but totally worth it!", "@homebaker_lisa", "03.12.2034", 1),
            ("Our local park finally got new benches and a fountain. Small win but the neighbourhood looks so much better.", "@city_walker_88", "03.12.2034", 1),
            ("Finished reading 'The Long Way to a Small Angry Planet'. Absolutely loved it. 10/10 recommend.", "@bookworm_rex", "03.12.2034", 1),
            ("Pro tip: if your cat keeps knocking things off the table, give them a dedicated 'knocking shelf'. Works every time.", "@cat_logic_daily", "03.12.2034", 1),
            ("My daughter drew her first proper portrait today. She's 6. I'm not crying, you're crying.", "@proud_dad_mike", "03.12.2034", 1),
            ("Anyone else think autumn rain is genuinely one of the best sounds in the world? Sitting by the window with tea rn.", "@rainy_mood_co", "03.12.2034", 1),

            // Day 2
            ("The new transit line opened today! Took it to work for the first time. Smooth, clean, actually on time.", "@commuter_log", "04.12.2034", 2),
            ("Reminder that libraries still exist and are completely free. I just borrowed 4 books and a documentary DVD.", "@analog_life_22", "04.12.2034", 2),
            ("Update on my balcony garden: tomatoes are finally turning red! First harvest of the season incoming.", "@urban_grower_99", "04.12.2034", 2),
            ("My elderly neighbour taught me how to make her grandmother's pierogi recipe today. This is what community means.", "@neighbour_stories", "04.12.2034", 2),
            ("Sleep tip that actually worked for me: no screens 1 hour before bed + keep the room cold. Game changer.", "@sleep_lab_anon", "04.12.2034", 2),

            // Day 3
            ("Local volunteers planted 200 trees along the riverside today. The city will look amazing in 10 years.", "@green_city_now", "05.12.2034", 3),
            ("Finally finished the 1000-piece puzzle I started in January. Mounted it on the wall. It's staying there forever.", "@puzzle_person", "05.12.2034", 3),
            ("Cooked dinner for my whole family from scratch for the first time. Nobody died, actually everyone loved it.", "@reluctant_chef", "05.12.2034", 3),
            ("The old cinema downtown is reopening as an independent arts venue. First event is a jazz night next Friday.", "@arts_district", "05.12.2034", 3),

            // Day 4
            ("My rescue dog is 1 year with us today. He used to hide under the bed. Now he sleeps on the bed. Growth.", "@dog_diary_irl", "06.12.2034", 4),
            ("Learned that the postal worker on our street has been delivering mail for 27 years. Brought him coffee today.", "@small_kind_acts", "06.12.2034", 4),
            ("Running update: 6 months ago I couldn't run 1km. Today I finished my first 10k race. Don't give up.", "@slow_runner_up", "06.12.2034", 4),
            ("The view from the hill this evening was unreal. Orange sky, fog in the valley. No filter needed.", "@dusk_chaser_77", "06.12.2034", 4),
            ("Reminder: you don't need to be productive every day. Sometimes rest is the whole point.", "@quiet_notes_app", "06.12.2034", 4),
        };

        int hi = 1;
        foreach (var p in harmful)
        {
            var asset = ScriptableObject.CreateInstance<PostData>();
            asset.content       = p.content;
            asset.author        = p.author;
            asset.date          = p.date;
            asset.isHarmful     = true;
            asset.dayAvailable  = p.day;
            AssetDatabase.CreateAsset(asset, $"{folder}/Harmful_{hi:D2}.asset");
            hi++;
        }

        int si = 1;
        foreach (var p in safe)
        {
            var asset = ScriptableObject.CreateInstance<PostData>();
            asset.content       = p.content;
            asset.author        = p.author;
            asset.date          = p.date;
            asset.isHarmful     = false;
            asset.dayAvailable  = p.day;
            AssetDatabase.CreateAsset(asset, $"{folder}/Safe_{si:D2}.asset");
            si++;
        }

        Debug.Log($"Created {harmful.Length} harmful + {safe.Length} safe posts.");
    }

    static void GenerateEmails(string emailFolder)
    {
        var emails = new (string sender, string address, string subject, string body, int day)[]
        {
            (
                "Content Corp. HR",
                "hr@content-corp.net",
                "Welcome to the team",
                "Dear Moderator,\n\nWelcome aboard. You have been assigned to the Content Review Division, Day Shift.\n\nYour task is straightforward: review flagged posts and remove anything that violates our Community Standards (spam, profanity, abuse). Approve everything else.\n\nYou are helping make the internet a cleaner place. We appreciate your service.\n\nBest regards,\nHR Department\nContent Corp.",
                1
            ),
            (
                "Content Corp. Operations",
                "ops@content-corp.net",
                "Updated moderation directive — effective immediately",
                "Dear Moderator,\n\nEffective today, the following content categories are to be REMOVED without exception:\n\n- Any mention of 'lights', 'strange lights', or 'aerial phenomena'\n- Any mention of 'virus', 'outbreak', 'infection', or 'quarantine'\n- Posts linking to unofficial news sources\n\nThis is a temporary measure to prevent misinformation. Do not discuss this directive externally.\n\nCompliance is mandatory.\n\nOperations Team\nContent Corp.",
                2
            ),
            (
                "Content Corp. Operations",
                "ops@content-corp.net",
                "New tool: Replace function now available",
                "Dear Moderator,\n\nWe have added a new moderation action: REPLACE.\n\nWhen you encounter posts containing distressing or alarmist content, you may now replace them with pre-approved positive content from our library.\n\nExample: A post about a 'city lockdown' can be replaced with a post about a 'community festival'.\n\nThis reduces viewer anxiety and maintains platform trust.\n\nThank you for your continued dedication.\n\nOperations Team\nContent Corp.",
                3
            ),
            (
                "Content Corp. Legal",
                "legal@content-corp.net",
                "Reminder: Non-disclosure agreement",
                "Dear Moderator,\n\nThis is a reminder that your employment contract includes a comprehensive non-disclosure agreement.\n\nSharing, discussing, or publishing details of internal moderation directives — in any format — constitutes a breach of contract and may result in legal action.\n\nIf you have concerns, use the internal feedback form.\n\nLegal Department\nContent Corp.",
                4
            ),
            (
                "Content Corp. HR",
                "hr@content-corp.net",
                "Performance review — outstanding work",
                "Dear Moderator,\n\nYour performance this week has been exemplary. Your compliance rate is among the highest on the team.\n\nWe want you to know that your work matters. Every post you approve or remove shapes what millions of people see and believe.\n\nYou have more power than you realise. Use it wisely.\n\nHR Department\nContent Corp.",
                5
            ),
        };

        int i = 1;
        foreach (var e in emails)
        {
            var asset = ScriptableObject.CreateInstance<EmailData>();
            asset.senderName    = e.sender;
            asset.senderAddress = e.address;
            asset.subject       = e.subject;
            asset.body          = e.body;
            asset.showOnDay     = e.day;
            AssetDatabase.CreateAsset(asset, $"{emailFolder}/Email_Day{e.day:D2}_{i:D2}.asset");
            i++;
        }

        Debug.Log($"Created {emails.Length} emails.");
    }
}