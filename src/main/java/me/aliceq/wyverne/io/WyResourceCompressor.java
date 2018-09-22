/*
 * The MIT License
 *
 * Copyright 2018 Alice Quiros <email@aliceq.me>.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
package me.aliceq.wyverne.io;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;
import me.aliceq.wyverne.resources.IWySerializableResource;

/**
 * Serializes a resource into a byte stream
 *
 * @author Alice Quiros <email@aliceq.me>
 */
public class WyResourceCompressor implements AutoCloseable {

    ZipOutputStream zipStream;

    /**
     * Creates a compressor that writes to the specified output stream
     *
     * @param output Output stream to write to
     * @throws IOException
     */
    public WyResourceCompressor(OutputStream output) throws IOException {
        this.zipStream = new ZipOutputStream(output);
    }

    /**
     * Digests a resource by converting it to a byte stream, compressing it, and
     * writing it to the output
     *
     * @param resource
     * @return The total number of bytes written
     * @throws IOException
     */
    public long digest(IWySerializableResource resource) throws IOException {
        InputStream input = resource.toByteStream();

        ZipEntry entry = new ZipEntry(resource.resourceId());
        this.zipStream.putNextEntry(entry);

        try {
            long totalBytes = 0;
            while (true) {
                int bytes = input.available();
                if (bytes <= 0) {
                    break;
                }

                byte[] buff = new byte[bytes];
                bytes = input.read(buff, 0, bytes);
                this.zipStream.write(buff);

                totalBytes += bytes;
            }

            return totalBytes;
        } finally {
            this.zipStream.closeEntry();
        }
    }

    @Override
    public void close() throws Exception {
        this.zipStream.close();
    }
}
