namespace AshLake.Contracts.Seedwork.Boorus;

public class Danbooru : Booru
{
    public const string Alias = "danbooru";
    public class PostMetadataKeys
    {
        public const string id = "id";
        public const string created_at = "created_at";
        public const string uploader_id = "uploader_id";
        public const string score = "score";
        public const string source = "source";
        public const string md5 = "md5";
        public const string last_comment_bumped_at = "last_comment_bumped_at";
        public const string rating = "rating";
        public const string image_width = "image_width";
        public const string image_height = "image_height";
        public const string tag_string = "tag_string";
        public const string fav_count = "fav_count";
        public const string file_ext = "file_ext";
        public const string last_noted_at = "last_noted_at";
        public const string parent_id = "parent_id";
        public const string has_children = "has_children";
        public const string approver_id = "approver_id";
        public const string tag_count_general = "tag_count_general";
        public const string tag_count_artist = "tag_count_artist";
        public const string tag_count_character = "tag_count_character";
        public const string tag_count_copyright = "tag_count_copyright";
        public const string file_size = "file_size";
        public const string up_score = "up_score";
        public const string down_score = "down_score";
        public const string is_pending = "is_pending";
        public const string is_flagged = "is_flagged";
        public const string is_deleted = "is_deleted";
        public const string tag_count = "tag_count";
        public const string updated_at = "updated_at";
        public const string is_banned = "is_banned";
        public const string pixiv_id = "pixiv_id";
        public const string last_commented_at = "last_commented_at";
        public const string has_active_children = "has_active_children";
        public const string bit_flags = "bit_flags";
        public const string tag_count_meta = "tag_count_meta";
        public const string has_large = "has_large";
        public const string has_visible_children = "has_visible_children";
        public const string tag_string_general = "tag_string_general";
        public const string tag_string_character = "tag_string_character";
        public const string tag_string_copyright = "tag_string_copyright";
        public const string tag_string_artist = "tag_string_artist";
        public const string tag_string_meta = "tag_string_meta";
        public const string file_url = "file_url";
        public const string large_file_url = "large_file_url";
        public const string preview_file_url = "preview_file_url";
    }
}
