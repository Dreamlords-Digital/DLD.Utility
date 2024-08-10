DLD.Serializer

The main class used here is JsonFxTextDataIO, a serializer/deserializer using a specific version of JsonFx. It provides:
1. multithreaded loading of json files in a folder
2. a lenient type coercion (even if class was renamed, moved to a different namespace or assembly, it will still deserialize)
3. proper deserialization of polymorphic lists (so a list of base class but with various different derived classes as its elements will serialize/deserialize properly)
4. a hash checksum creator (fields of a class/struct that are to be serialized will be hashed, giving you a checksum of the serialized data to see if anything has changed since then)

There are also tests to ensure it serializes/deserializes basic Unity types (Vector3, Color, Quaternion, etc).