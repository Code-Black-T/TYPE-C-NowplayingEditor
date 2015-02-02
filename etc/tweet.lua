-- Luaの初期化の時に一度だけ呼び出されます
-- Luaのスクリプトの編集の反映にはアプリケーションの再起動が必要になります
function main()
    console("Lua scripting module initialized!!");
end

function OnTweet(Song, opt, isCustomTweet)
    -- Song:中には曲情報が入っています。iTunesClass.csを参考に弄ってください
    -- opt:ツイートです。中身を変える時はopt.Statusの中身を変更でお願いします
    -- 何もしない時はとりあえずtrueを返すようになっています(falseにすると投稿自体を取り消します)
    
    -- カスタムツイートの時は何もしない
    -- if isCustomTweet == true then
    --     return true;
    -- end

	if Song.SongRating == 0 then
		RateStar = "評価なし";
	end

	if Song.SongRating == 20 then
		RateStar = "★☆☆☆☆";
	end

	if Song.SongRating == 40 then
		RateStar = "★★☆☆☆";
	end

	if Song.SongRating == 60 then
		RateStar = "★★★☆☆";
	end

	if Song.SongRating == 80 then
		RateStar = "★★★★☆";
	end

	if Song.SongRating == 100 then
		RateStar = "★★★★★";
	end

	opt.Status = string.gsub(opt.Status,"$RATESTAR","レート："..RateStar)

    return true;
end