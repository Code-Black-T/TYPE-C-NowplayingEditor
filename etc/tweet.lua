-- Lua�̏������̎��Ɉ�x�����Ăяo����܂�
-- Lua�̃X�N���v�g�̕ҏW�̔��f�ɂ̓A�v���P�[�V�����̍ċN�����K�v�ɂȂ�܂�
function main()
    console("Lua scripting module initialized!!");
end

function OnTweet(Song, opt, isCustomTweet)
    -- Song:���ɂ͋ȏ�񂪓����Ă��܂��BiTunesClass.cs���Q�l�ɘM���Ă�������
    -- opt:�c�C�[�g�ł��B���g��ς��鎞��opt.Status�̒��g��ύX�ł��肢���܂�
    -- �������Ȃ����͂Ƃ肠����true��Ԃ��悤�ɂȂ��Ă��܂�(false�ɂ���Ɠ��e���̂��������܂�)
    
    -- �J�X�^���c�C�[�g�̎��͉������Ȃ�
    -- if isCustomTweet == true then
    --     return true;
    -- end

	if Song.SongRating == 0 then
		RateStar = "�]���Ȃ�";
	end

	if Song.SongRating == 20 then
		RateStar = "����������";
	end

	if Song.SongRating == 40 then
		RateStar = "����������";
	end

	if Song.SongRating == 60 then
		RateStar = "����������";
	end

	if Song.SongRating == 80 then
		RateStar = "����������";
	end

	if Song.SongRating == 100 then
		RateStar = "����������";
	end

	opt.Status = string.gsub(opt.Status,"$RATESTAR","���[�g�F"..RateStar)

    return true;
end