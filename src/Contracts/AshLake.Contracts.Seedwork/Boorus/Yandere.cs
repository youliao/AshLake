﻿namespace AshLake.Contracts.Seedwork.Boorus;

public class Yandere : Booru
{
    public const string Alias = "yandere";

    public class PostMetadataKeys
    {
        public const string id = "id";
        public const string tags = "tags";
        public const string created_at = "created_at";
        public const string updated_at = "updated_at";
        public const string creator_id = "creator_id";
        public const string approver_id = "approver_id";
        public const string author = "author";
        public const string change = "change";
        public const string source = "source";
        public const string score = "score";
        public const string md5 = "md5";
        public const string file_size = "file_size";
        public const string file_ext = "file_ext";
        public const string file_url = "file_url";
        public const string is_shown_in_index = "is_shown_in_index";
        public const string preview_url = "preview_url";
        public const string preview_width = "preview_width";
        public const string preview_height = "preview_height";
        public const string actual_preview_width = "actual_preview_width";
        public const string actual_preview_height = "actual_preview_height";
        public const string sample_url = "sample_url";
        public const string sample_width = "sample_width";
        public const string sample_height = "sample_height";
        public const string sample_file_size = "sample_file_size";
        public const string jpeg_url = "jpeg_url";
        public const string jpeg_width = "jpeg_width";
        public const string jpeg_height = "jpeg_height";
        public const string jpeg_file_size = "jpeg_file_size";
        public const string rating = "rating";
        public const string is_rating_locked = "is_rating_locked";
        public const string has_children = "has_children";
        public const string parent_id = "parent_id";
        public const string status = "status";
        public const string is_pending = "is_pending";
        public const string width = "width";
        public const string height = "height";
        public const string is_held = "is_held";
        public const string frames_pending_string = "frames_pending_string";
        public const string frames_pending = "frames_pending";
        public const string frames_string = "frames_string";
        public const string frames = "frames";
        public const string is_note_locked = "is_note_locked";
        public const string last_noted_at = "last_noted_at";
        public const string last_commented_at = "last_commented_at";
    }

    public class TagMetadataKeys
    {
        public const string id = "id";
        public const string name = "name";
        public const string count = "count";
        public const string type = "type";
        public const string ambiguous = "ambiguous";
    }
}
